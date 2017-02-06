using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Control;

namespace LNF.Control
{
    public enum LocationType
    {
        Internal = 2,
        External = 3
    }

    [Serializable]
    public class InstanceAuthorization
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public int? Point { get; set; }
        public int? ActionID { get; set; }
        public int? BlockID { get; set; }
        public string BlockName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public static IList<InstanceAuthorization> Create(Client client, string location, LocationType locationType, string actionName)
        {
            IList<InstanceAuthorization> result = new List<InstanceAuthorization>();

            if (actionName == "ByPass")
                actionName = "AlarmByPass";

            ControlAction action = DA.Current.Query<ControlAction>().FirstOrDefault(x => x.ActionName == actionName);
            if (action == null)
                throw new Exception(string.Format("Unable to find action: {0}", actionName));

            string[] locs = { "1", location, ((int)locationType).ToString() };

            IList<ControlAuthorization> query = DA.Current.Query<ControlAuthorization>().ToList();
            ControlAuthorization[] auths = query
                .Where(x => (x.ClientID == -1 * client.ClientID || (x.ClientID & (int)client.Privs) > 0) && locs.Contains(x.Location) && x.ActionID == action.ActionID).ToArray();

            foreach (ControlAuthorization ca in auths)
            {
                IList<ActionInstance> act = DA.Current.Query<ActionInstance>().Where(x => x.ActionName == actionName && ca.ActionInstanceID == x.Index).ToList();
                if (act.Count > 0)
                {
                    foreach (ActionInstance inst in act)
                    {
                        Point p = inst.GetPoint();
                        result.Add(new InstanceAuthorization()
                        {
                            Index = ca.ActionInstanceID,
                            Name = inst.Name,
                            Point = inst.Point,
                            ActionID = inst.ActionID,
                            BlockID = p.Block.BlockID,
                            BlockName = p.Block.BlockName,
                            Description = p.Block.Description,
                            Status = null
                        });
                    }
                }
                else
                {
                    result.Add(new InstanceAuthorization()
                    {
                        Index = ca.ActionInstanceID,
                        Name = null,
                        Point = null,
                        ActionID = null,
                        BlockID = null,
                        BlockName = null,
                        Description = null,
                        Status = null
                    });
                }
            }

            return result;
        }

        public static IList<InstanceAuthorization> Create(string actionName)
        {
            IList<InstanceAuthorization> result = new List<InstanceAuthorization>();

            IList<ActionInstance> act = DA.Current.Query<ActionInstance>().Where(x => x.ActionName == actionName).ToList();
            foreach(ActionInstance inst in act)
            {
                Point p = inst.GetPoint();
                result.Add(new InstanceAuthorization()
                {
                    Index = inst.Index,
                    Name = inst.Name,
                    Point = inst.Point,
                    ActionID = inst.ActionID,
                    BlockID = p.Block.BlockID,
                    BlockName = p.Block.BlockName,
                    Description = p.Block.Description,
                    Status = null
                });
            }

            return result;
        }
    }
}
