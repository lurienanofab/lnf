using LNF.Control;
using LNF.Repository;
using LNF.Repository.Control;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    public struct InterlockData
    {
        public int BlockID { get; }
        public BlockState BlockState { get; }
        public string Message { get; }

        public InterlockData(int blockId, BlockState state, string message)
        {
            BlockID = blockId;
            BlockState = state;
            Message = message;
        }
    }

    public static class WagoInterlock
    {
        // Turn On/Off the points that are associated with the resource
        public static bool ToggleInterlock(int resourceId, bool state, uint duration)
        {
            // Not every tool has an interlock setup, so only set point state if
            // an ActionInstance is found. It's not an error if one is not found.

            ActionInstance inst = ActionInstanceUtility.Find(ActionType.Interlock, resourceId);

            if (inst != null)
            {
                ServiceProvider.Current.Control.SetPointState(inst.GetPoint(), state, duration);
                return true;
            }
            else
            { 
                return false;
            }
        }

        public static void ByPass(int actionId, uint duration)
        {
            ActionInstance inst = ActionInstanceUtility.Find(ActionType.ByPass, actionId);

            if (inst != null)
                ServiceProvider.Current.Control.SetPointState(inst.GetPoint(), true, duration);
        }

        // Get Block data
        public static InterlockData GetInterlockData(int resourceId, ActionType action = ActionType.Interlock, string username = "", string password = "")
        {
            ActionInstance inst = ActionInstanceUtility.Find(action, resourceId);

            BlockState blockState = null;

            if (inst == null)
                return new InterlockData(0, null, "No Interlock is associated with this resource.");

            Point point = inst.GetPoint();

            string message = string.Empty;

            int blockId = 0;

            try
            {
                blockId = point.Block.BlockID;
                BlockResponse resp = ServiceProvider.Current.Control.GetBlockState(point.Block);
                blockState = resp.BlockState;

                if (blockState == null)
                    message = "Failed to connect with WAGO block.<br />Please try to reload the page and repeat your action again (click the Maintenance tab above).<br />If the problem persists, please contact administrator.";
            }
            catch (Exception ex)
            {
                message = string.Format("Wago block {0} fault detected. Please contact system administrator. [{0}]", blockId, ex.Message);
            }

            return new InterlockData(blockId, blockState, message);
        }

        public static void AllToolStatus(DataTable dtToolStatus)
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

            Block[] blocks = DA.Current.Query<Block>().ToArray();
            Dictionary<int, BlockState> dict = new Dictionary<int, BlockState>();
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
                        ActionInstance inst = ActionInstanceUtility.Find(ActionType.Interlock, row.Field<int>("ResourceID"));
                        if (inst != null)
                            dr.SetField("PointID", inst.Point);
                        else
                            dr.SetField("PointID", 0);
                    }

                    int pointId = dr.Field<int>("PointID");

                    if (pointId > 0)
                    {
                        Point point = DA.Current.Query<Point>().FirstOrDefault(x => x.PointID == pointId);

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
                                BlockResponse resp = ServiceProvider.Current.Control.GetBlockState(blocks.First(x => x.BlockID == blockId));
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
                            bool pointState = GetPointState(point.PointID, blockState);
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

        public static BlockState GetBlockState(int blockId)
        {
            Block block = DA.Current.Query<Block>().First(x => x.BlockID == blockId);
            BlockResponse resp = ServiceProvider.Current.Control.GetBlockState(block);
            return resp.EnsureSuccess().BlockState;
        }

        // Parses the passed data buffer, and returns the state of a point(word, bit) on the block
        public static bool GetPointState(int pointId, BlockState blockState)
        {
            PointState ps = blockState.Points.First(x => x.PointID == pointId);
            return ps.State;
        }

        public static bool GetPointState(int resourceId)
        {
            ActionInstance inst = ActionInstanceUtility.Find(ActionType.Interlock, resourceId);

            if (inst == null)
                throw new ArgumentException(string.Format("No record found for ResourceID {0}", resourceId));

            Point p = inst.GetPoint();

            var bs = GetBlockState(p.Block.BlockID);

            return GetPointState(p.PointID, bs);
        }

        public static int GetAnalogPointState(ActionInstance inst, BlockState blockState = null)
        {
            throw new NotImplementedException();
        }
    }
}
