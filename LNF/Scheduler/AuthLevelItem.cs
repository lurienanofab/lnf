namespace LNF.Scheduler
{
    public class AuthLevelItem : IAuthLevel
    {
        public ClientAuthLevel AuthLevelID { get; set; }
        public int Authorizable { get; set; }
        public string AuthLevelName { get; set; }
    }
}
