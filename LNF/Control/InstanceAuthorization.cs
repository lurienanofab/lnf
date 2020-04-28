using LNF.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public string Status { get; set; }

        public static IList<InstanceAuthorization> Create(IClient client, string location, LocationType locationType, string actionName)
        {
            IList<InstanceAuthorization> result = new List<InstanceAuthorization>();

            if (actionName == "ByPass")
                actionName = "AlarmByPass";

            IControlAction action = ServiceProvider.Current.Control.GetControlAction(actionName); 

            if (action == null)
                throw new Exception(string.Format("Unable to find action: {0}", actionName));

            string[] locs = { "1", location, ((int)locationType).ToString() };

            IList<IControlAuthorization> query = ServiceProvider.Current.Control.GetControlAuthorizations().ToList();

            IControlAuthorization[] auths = query
                .Where(x => (x.ClientID == -1 * client.ClientID || (x.ClientID & (int)client.Privs) > 0) && locs.Contains(x.Location) && x.ActionID == action.ActionID).ToArray();

            foreach (IControlAuthorization ca in auths)
            {
                IList<IActionInstance> act = ServiceProvider.Current.Control.GetActionInstances(actionName, ca.ActionInstanceID).ToList();

                if (act.Count > 0)
                {
                    foreach (IActionInstance inst in act)
                    {
                        IPoint p = inst.GetPoint();
                        result.Add(new InstanceAuthorization()
                        {
                            Index = ca.ActionInstanceID,
                            Name = inst.Name,
                            Point = inst.Point,
                            ActionID = inst.ActionID,
                            BlockID = p.BlockID,
                            BlockName = p.BlockName,
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
                        Status = null
                    });
                }
            }

            return result;
        }

        public static IList<InstanceAuthorization> Create(string actionName)
        {
            IList<InstanceAuthorization> result = new List<InstanceAuthorization>();

            IList<IActionInstance> act = ServiceProvider.Current.Control.GetActionInstances(actionName).ToList();

            foreach(IActionInstance inst in act)
            {
                IPoint p = inst.GetPoint();

                result.Add(new InstanceAuthorization()
                {
                    Index = inst.Index,
                    Name = inst.Name,
                    Point = inst.Point,
                    ActionID = inst.ActionID,
                    BlockID = p.BlockID,
                    BlockName = p.BlockName,
                    Status = null
                });
            }

            return result;
        }
    }
}
