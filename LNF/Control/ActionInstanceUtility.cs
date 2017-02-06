using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Control;

namespace LNF.Control
{
    public enum ActionType
    {
        Interlock = 1,
        ByPass = 2
    }

    public static class ActionInstanceUtility
    {
        public static ActionInstance Find(ActionType action, int actionId)
        {
            ActionInstance result = DA.Current.Query<ActionInstance>().FirstOrDefault(x => x.ActionName == Enum.GetName(typeof(ActionType), action) && x.ActionID == actionId);
            return result;
        }

        public static Point GetPoint(ActionInstance instance)
        {
            if (instance == null) return null;
            Point result = DA.Current.Single<Point>(instance.Point);
            return result;
        }
    }
}
