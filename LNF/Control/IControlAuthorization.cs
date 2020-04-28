namespace LNF.Control
{
    public interface IControlAuthorization
    {
        int ActionID { get; set; }
        int ActionInstanceID { get; set; }
        int ClientID { get; set; }
        string Location { get; set; }
    }
}
