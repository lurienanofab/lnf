using System;

namespace LNF.Data
{
    public interface IStaffDirectory
    {
        int ClientID { get; set; }
        string LName { get; set; }
        string FName { get; set; }
        string DisplayName { get; }
        string ContactPhone { get; set; }
        string ContactEmail { get; set; }
        bool Deleted { get; set; }
        string HoursXML { get; set; }
        DateTime LastUpdate { get; set; }
        string Office { get; set; }
        int StaffDirectoryID { get; set; }
    }
}