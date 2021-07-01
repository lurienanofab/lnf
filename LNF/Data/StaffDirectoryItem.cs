using System;

namespace LNF.Data
{
    public class StaffDirectoryItem : IStaffDirectory
    {
        public int StaffDirectoryID { get; set; }
        public int ClientID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public string ContactEmail { get; set; }
        public string HoursXML { get; set; }
        public string ContactPhone { get; set; }
        public string Office { get; set; }
        public bool Deleted { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
