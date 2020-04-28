namespace LNF.Scheduler
{
    public interface IAuthLevel
    {
        ClientAuthLevel AuthLevelID { get; set; }
        int Authorizable { get; set; }
        string AuthLevelName { get; set; }
    }
}
