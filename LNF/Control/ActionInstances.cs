namespace LNF.Control
{
    public static class ActionInstances
    {
        public static IActionInstance Find(ActionType action, int actionId)
        {
            return ServiceProvider.Current.Control.GetActionInstance(action, actionId);
        }

        public static IPoint GetPoint(IActionInstance instance)
        {
            if (instance == null) return null;
            return ServiceProvider.Current.Control.GetPoint(instance.Point);
        }
    }
}
