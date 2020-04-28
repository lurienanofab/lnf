namespace LNF.Scheduler
{
    public interface IOnTheFlyResource
    {
        int OnTheFlyResourceID { get; set; }

        /// <summary>
        /// This is the unique 3 characters assigned to each RFID reader.
        /// </summary>
        string CardReaderName { get; set; }                     

        int ButtonIndex { get; set; }

        /// <summary>
        /// All CardReaderNames and matching ResourceTypes will be the same.
        /// </summary>
        OnTheFlyResourceType ResourceType { get; set; }

        int ResourceID { get; set; }

        int ResourceStateDuration { get; set; }

        /// <summary>
        /// Action can be CREATE_AND_START_RESERVATION or START_EXISTING_RESERVATION.
        /// </summary>
        OnTheFlyCardSwipeAction CardSwipeAction { get; set; }
    }
}
