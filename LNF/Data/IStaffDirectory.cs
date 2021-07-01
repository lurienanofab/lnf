using System;

namespace LNF.Data
{
    public interface IStaffDirectory
    {
        int StaffDirectoryID { get; set; }
        int ClientID { get; set; }
        string LName { get; set; }
        string FName { get; set; }
        string DisplayName { get; }
        string ContactEmail { get; set; }
        string HoursXML { get; set; }
        string ContactPhone { get; set; }
        string Office { get; set; }
        bool Deleted { get; set; }
        DateTime LastUpdate { get; set; }
    }
}