using System;

namespace LNF.Data
{
    public interface IClientOrg
    {
        int ClientOrgID { get; set; }
        int ClientAddressID { get; set; }
        string Phone { get; set; }
        string Email { get; set; }
        bool IsManager { get; set; }
        bool IsFinManager { get; set; }
        DateTime? SubsidyStartDate { get; set; }
        DateTime? NewFacultyStartDate { get; set; }
        bool ClientOrgActive { get; set; }
    }
}
