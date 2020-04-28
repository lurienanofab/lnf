namespace LNF.Scheduler
{
    public class OnTheFlyResourceItem : IOnTheFlyResource
    {
        public int OnTheFlyResourceID { get; set; }
        public string CardReaderName { get; set; }
        public int ButtonIndex { get; set; }
        public OnTheFlyResourceType ResourceType { get; set; }
        public int ResourceID { get; set; }
        public int ResourceStateDuration { get; set; }
        public OnTheFlyCardSwipeAction CardSwipeAction { get; set; }
    }
}
