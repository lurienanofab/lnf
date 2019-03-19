namespace LNF.Repository.Scheduler
{
    /*
     * The combination of (CardReaderName AND ButtonIndex) provides unique Resource
     * ButtonIndex is for Cabins and can be 1,2,3,4.   -1 is for Resource  
     * ResourceType is currently of type General ('resource':1) and ('cabin':2) 
     */

    public class OnTheFlyResource : IDataItem
    {
        public virtual int OnTheFlyResourceID { get; set; }
        public virtual string CardReaderName { get; set; }               /* This is the unique 3 characters assigned to each RFID reader */
        public virtual int ButtonIndex { get; set; }
        public virtual OnTheFlyResourceType ResourceType { get; set; }       /* All CardReaderNames and matching ResourceType will be same   */
        public virtual Resource Resource { get; set; }
        public virtual int ResourceStateDuration { get; set; }
        public virtual OnTheFlyCardSwipeAction CardSwipeAction { get; set; } /* Action can be CREATE_AND_START_RESERVATION OR START_EXISTING_RESERVATION */

        public virtual bool IsTool()
        {
            if (OnTheFlyResourceType.Tool == ResourceType)
                return true;
            return false;
        }
        public virtual bool IsCabinet()
        {
            if (OnTheFlyResourceType.Cabinet == ResourceType)
                return true;
            return false;
        }
        public virtual string GetResourceTypeAsString()
        {
            if (IsTool())
                return "tool";
            else if (IsCabinet())
                return "cabinet";

            return "-";
        }
        public virtual bool IsCreateAndStart()
        {
            if (OnTheFlyCardSwipeAction.CreateAndStartReservation == CardSwipeAction)
                return true;
            return false;
        }
    }
}