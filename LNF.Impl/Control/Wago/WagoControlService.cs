using LNF.Control;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Control;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Control.Wago
{
    public class WagoControlService : RepositoryBase, IControlService
    {
        private RestClient _client;

        public Uri Host { get; }

        public WagoControlService(ISessionManager mgr) : base(mgr)
        {
            if (Configuration.Current.Control.ElementInformation.IsPresent)
                Host = Configuration.Current.Control.Host;
        }

        public BlockResponse GetBlockState(int blockId)
        {
            var request = new RestRequest("wago/block/{blockId}");
            request.AddUrlSegment("blockId", blockId);
            return GetSuccessfulResult<BlockResponse>(request);
        }

        public bool GetPointState(int pointId)
        {
            var point = GetExistingPoint(pointId);
            var blockResult = GetBlockState(point.Block.BlockID);
            var result = blockResult.BlockState.GetPointState(pointId);
            return result;
        }

        public IEnumerable<IToolStatus> GetToolStatus()
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString))
            using (var cmd = conn.CreateCommand("SELECT * FROM sselControl.dbo.v_ToolStatus WHERE IsActive = 1 ORDER BY BuildingName, LabDisplayName, ProcessTechName, ResourceName", CommandType.Text))
            using (var adap = new SqlDataAdapter(cmd))
            {
                adap.Fill(dt);
                conn.Close();
            }

            FillToolStatusTable(dt);

            var result = dt.AsEnumerable().Select(x => new ToolStatusItem()
            {
                BuildingID = x.Field<int>("BuildingID"),
                BuildingName = x.Field<string>("BuildingName"),
                LabID = x.Field<int>("LabID"),
                LabName = x.Field<string>("LabName"),
                LabDisplayName = x.Field<string>("LabDisplayName"),
                ProcessTechID = x.Field<int>("ProcessTechID"),
                ProcessTechName = x.Field<string>("ProcessTechName"),
                ResourceID = x.Field<int>("ResourceID"),
                ResourceName = x.Field<string>("ResourceName"),
                PointID = x.Field<int>("PointID"),
                InterlockStatus = x.Field<string>("InterlockStatus"),
                InterlockState = x.Field<bool>("InterlockState"),
                InterlockError = x.Field<bool>("InterlockError"),
                IsInterlocked = x.Field<bool>("IsInterlocked")
            }).ToList();

            return result;
        }

        public IBlockConfig GetBlockConfig(int blockId, byte modPosition)
        {
            return Session.Query<BlockConfig>()
                .FirstOrDefault(x => x.BlockID == blockId && x.ModPosition == modPosition);
        }

        private void FillToolStatusTable(DataTable dtToolStatus)
        {
            DataRow row = null;

            if (!dtToolStatus.Columns.Contains("PointID"))
                dtToolStatus.Columns.Add("PointID", typeof(int));
            if (!dtToolStatus.Columns.Contains("InterlockStatus"))
                dtToolStatus.Columns.Add("InterlockStatus", typeof(string));
            if (!dtToolStatus.Columns.Contains("InterlockState"))
                dtToolStatus.Columns.Add("InterlockState", typeof(bool));
            if (!dtToolStatus.Columns.Contains("InterlockError"))
                dtToolStatus.Columns.Add("InterlockError", typeof(bool));
            if (!dtToolStatus.Columns.Contains("IsInterlocked"))
                dtToolStatus.Columns.Add("IsInterlocked", typeof(bool));

            int blockId = 0;
            BlockState blockState;

            var blocks = Session.Query<Block>().ToList();
            Dictionary<int, BlockState> dict = new Dictionary<int, BlockState>();

            var points = Session.Query<Point>().ToList();
            var interlocks = Session.Query<ActionInstance>().Where(x => x.ActionName == "Interlock").ToList();

            foreach (DataRow dr in dtToolStatus.Rows)
            {
                row = dr;
                blockState = null;

                if (dr["ResourceID"] != DBNull.Value)
                {
                    dr.SetField("InterlockStatus", "Blank");
                    dr.SetField("InterlockState", false);
                    dr.SetField("InterlockError", false);
                    dr.SetField("IsInterlocked", true);

                    if (dr["PointID"] == DBNull.Value)
                    {
                        var inst = interlocks.FirstOrDefault(x => x.ActionID == row.Field<int>("ResourceID"));

                        if (inst != null)
                            dr.SetField("PointID", inst.Point);
                        else
                            dr.SetField("PointID", 0);
                    }

                    int pointId = dr.Field<int>("PointID");

                    if (pointId > 0)
                    {
                        Point point = points.First(x => x.PointID == pointId);

                        blockId = point.Block.BlockID;

                        if (dict.ContainsKey(blockId))
                        {
                            blockState = dict[blockId];

                            if (blockState == null)
                            {
                                row.SetField("InterlockStatus", "Wago block fault detected.");
                                row.SetField("InterlockError", true);
                            }
                        }
                        else
                        {
                            //We try to get returned data
                            try
                            {
                                BlockResponse resp = GetBlockState(blockId);
                                blockState = resp.EnsureSuccess().BlockState;
                            }
                            catch (Exception ex)
                            {
                                string errmsg = ex.ToString();
                                row.SetField("InterlockStatus", "Wago block fault detected.");
                                row.SetField("InterlockError", true);
                            }

                            //blockState might be null here if there was a fault but that's ok
                            dict.Add(blockId, blockState);
                        }

                        if (blockState != null)
                        {
                            bool pointState = blockState.GetPointState(point.PointID);
                            row.SetField("InterlockState", pointState);

                            if (pointState)
                                row.SetField("InterlockStatus", "Tool Enabled");
                            else
                                row.SetField("InterlockStatus", "Tool Disabled");
                        }

                    }
                    else
                    {
                        //this is not an error
                        row.SetField("InterlockStatus", "No interlock for resource.");
                        row.SetField("IsInterlocked", false);
                    }
                }
            }
        }


        public IEnumerable<IActionInstance> GetActionInstances()
        {
            return Session.Query<ActionInstance>().ToList();
        }

        public IEnumerable<IActionInstance> GetActionInstances(string actionName)
        {
            return Session.Query<ActionInstance>().Where(x => x.ActionName == actionName).ToList();
        }

        public IEnumerable<IActionInstance> GetActionInstances(string actionName, int index)
        {
            return Session.Query<ActionInstance>().Where(x => x.ActionName == actionName && x.Index == index).ToList();
        }

        public IActionInstance GetActionInstance(ActionType action, int actionId)
        {
            string a = Enum.GetName(typeof(ActionType), action);
            var result = Session.Query<ActionInstance>().FirstOrDefault(x => x.ActionName == a && x.ActionID == actionId);
            return result;
        }

        public IActionInstance GetActionInstance(ActionType action, IPoint point)
        {
            var result = Session.Query<ActionInstance>().FirstOrDefault(x => x.ActionName == Enum.GetName(typeof(ActionType), action) && x.Point == point.PointID);
            return result;
        }

        public PointResponse SetPointState(int pointId, bool state, uint duration)
        {
            var point = GetExistingPoint(pointId);
            var request = new RestRequest("wago/point/{pointId}");
            request.AddUrlSegment("blockId", point.Block.BlockID);
            request.AddUrlSegment("pointId", point.PointID);
            request.AddParameter("state", state);
            request.AddParameter("duration", duration);
            return GetSuccessfulResult<PointResponse>(request);
        }

        public PointResponse Cancel(int pointId)
        {
            var point = GetExistingPoint(pointId);
            var request = new RestRequest("wago/point/{pointId}/cancel");
            request.AddUrlSegment("blockId", point.Block.BlockID);
            request.AddUrlSegment("pointId", point.PointID);
            return GetSuccessfulResult<PointResponse>(request);
        }

        public IBlock GetBlock(int blockId)
        {
            return Require<IBlock>(blockId);
        }

        public IEnumerable<IBlock> GetBlocks()
        {
            return Session.Query<Block>().ToList();
        }

        public IPoint GetPoint(int pointId)
        {
            return Require<PointInfo>(pointId);
        }

        public IControlAction GetControlAction(string actionName)
        {
            return Session.Query<ControlAction>().FirstOrDefault(x => x.ActionName == actionName);
        }

        public IEnumerable<IControlAuthorization> GetControlAuthorizations()
        {
            return Session.Query<ControlAuthorization>().ToList();
        }

        private Point GetExistingPoint(int pointId)
        {
            var result = Session.Get<Point>(pointId);

            if (result == null)
                throw new Exception($"Cannot find Point with PointID = {pointId}");

            return result;
        }

        private IRestClient GetClient()
        {
            if (_client == null)
            {
                if (Host == null)
                    throw new Exception("The control configuration element is missing.");

                _client = new RestClient(Host);
            }

            return _client;
        }

        private T GetSuccessfulResult<T>(IRestRequest request) where T : class, new()
        {
            var client = GetClient();
            var response = client.Execute<T>(request);

            if (response.IsSuccessful)
                return response.Data;
            else
                throw new InvalidOperationException($"[{(int)response.StatusCode}:{response.StatusCode} {client.BaseUrl}{request.Resource}]{Environment.NewLine}{response.ErrorMessage}");
        }

        public void Dispose()
        {
            _client = null;
        }
    }
}
