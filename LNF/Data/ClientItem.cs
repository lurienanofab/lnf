﻿using System;

namespace LNF.Data
{
    public class ClientItem : OrgItem, IClient
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string FName { get; set; }
        public string MName { get; set; }
        public string LName { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public ClientPrivilege Privs { get; set; }
        public int Communities { get; set; }
        public bool IsChecked { get; set; }
        public bool IsSafetyTest { get; set; }
        public bool RequirePasswordReset { get; set; }
        public bool ClientActive { get; set; }
        public int DemCitizenID { get; set; }
        public string DemCitizenName { get; set; }
        public int DemGenderID { get; set; }
        public string DemGenderName { get; set; }
        public int DemRaceID { get; set; }
        public string DemRaceName { get; set; }
        public int DemEthnicID { get; set; }
        public string DemEthnicName { get; set; }
        public int DemDisabilityID { get; set; }
        public string DemDisabilityName { get; set; }
        public int TechnicalInterestID { get; set; }
        public string TechnicalInterestName { get; set; }
        public int ClientOrgID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsManager { get; set; }
        public bool IsFinManager { get; set; }
        public DateTime? SubsidyStartDate { get; set; }
        public DateTime? NewFacultyStartDate { get; set; }
        public int ClientAddressID { get; set; }
        public bool ClientOrgActive { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int MaxChargeTypeID { get; set; }
        public string MaxChargeTypeName { get; set; }
        public long EmailRank { get; set; }
        public bool IsStaff() => this.HasPriv(ClientPrivilege.Staff);

        public override string ToString()
        {
            return Clients.GetDisplayName(LName, FName);
        }
    }
}
