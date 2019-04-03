namespace LNF.Models.Data
{
    public interface IPriv
    {
        ClientPrivilege PrivFlag { get; set; }
        string PrivType { get; set; }
        string PrivDescription { get; set; }
    }
}
