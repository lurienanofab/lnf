namespace LNF.Models.Data
{
    public interface IPrivileged
    {
        int ClientID { get; set; }
        string UserName { get; set; }
        ClientPrivilege Privs { get; set; }
    }
}
