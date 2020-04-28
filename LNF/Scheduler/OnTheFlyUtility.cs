namespace LNF.Scheduler
{
    public static class OnTheFlyUtility
    {
        public static uint GetStateDuration(int resourceId)
        {
            IOnTheFlyResource r = ServiceProvider.Current.Scheduler.Resource.GetOnTheFlyResource(resourceId);

            if (r != null && r.ResourceType == OnTheFlyResourceType.Cabinet)
                return r.ResourceStateDuration > 0 ? (uint)r.ResourceStateDuration : 0;

            return 0;
        }
    }
}
