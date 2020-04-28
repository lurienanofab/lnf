namespace LNF.Scheduler
{
    public interface IAvailableInvitee
    {
        int ClientID { get; set; }
        string LName { get; set; }
        string FName { get; set; }
        string DisplayName { get; set; }
    }
}
