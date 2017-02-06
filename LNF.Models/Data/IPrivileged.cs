namespace LNF.Models.Data
{
    public interface IPrivileged
    {
        int ClientID { get; set; }
        ClientPrivilege Privs { get; set; }
    }
}
