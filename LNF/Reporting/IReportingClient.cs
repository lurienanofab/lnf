namespace LNF.Reporting
{
    public interface IReportingClient
    {
        int ClientID { get; set; }
        string Email { get; set; }
        string FName { get; set; }
        bool IsFinManager { get; set; }
        bool IsInternal { get; set; }
        bool IsManager { get; set; }
        string LName { get; set; }
        string UserName { get; set; }
    }
}