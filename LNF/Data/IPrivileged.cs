namespace LNF.Data
{
    public interface IPrivileged
    {
        int ClientID { get; set; }
        string UserName { get; set; }
        string LName { get; set; }
        string MName { get; set; }
        string FName { get; set; }
        string DisplayName { get; }
        ClientPrivilege Privs { get; set; }
    }
}
