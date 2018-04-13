using LNF.Models.Data;

namespace LNF.Help
{
    public class StaffDirectoryItem : IPrivileged
    {
        public int StaffDirectoryID { get; set; }
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public string Name { get; set; }
        public string Hours { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Office { get; set; }
        public bool Deleted { get; set; }
        public bool ReadOnly { get; set; }
    }
}
