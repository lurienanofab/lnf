namespace LNF.Models.Data
{
    public class PrivItem : IPriv
    {
        public ClientPrivilege PrivFlag { get; set; }
        public string PrivType { get; set; }
        public string PrivDescription { get; set; }
    }
}
