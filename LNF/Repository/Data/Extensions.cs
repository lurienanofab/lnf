using LNF.Models.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Data
{
    public static class Extensions
    {
        public static ClientItem GetClientItem(this ClientOrgInfoBase item)
        {
            if (item == null) return null;
            return new ClientItem()
            {
                ClientID = item.ClientID,
                UserName = item.UserName,
                FName = item.FName,
                MName = item.MName,
                LName = item.LName,
                DemCitizenID = item.DemCitizenID,
                DemCitizenName = item.DemCitizenName,
                DemGenderID = item.DemGenderID,
                DemGenderName = item.DemGenderName,
                DemRaceID = item.DemRaceID,
                DemRaceName = item.DemRaceName,
                DemEthnicID = item.DemEthnicID,
                DemEthnicName = item.DemEthnicName,
                DemDisabilityID = item.DemDisabilityID,
                DemDisabilityName = item.DemDisabilityName,
                Privs = item.Privs,
                Communities = item.Communities,
                TechnicalInterestID = item.TechnicalInterestID,
                TechnicalInterestName = item.TechnicalInterestName,
                IsChecked = item.IsChecked,
                IsSafetyTest = item.IsSafetyTest,
                ClientActive = item.ClientActive,
                ClientOrgID = item.ClientOrgID,
                Phone = item.Phone,
                Email = item.Email,
                IsManager = item.IsManager,
                IsFinManager = item.IsFinManager,
                SubsidyStartDate = item.SubsidyStartDate,
                NewFacultyStartDate = item.NewFacultyStartDate,
                ClientAddressID = item.ClientAddressID,
                DepartmentID = item.DepartmentID,
                DepartmentName = item.DepartmentName,
                RoleID = item.RoleID,
                RoleName = item.RoleName,
                ClientOrgActive = item.ClientOrgActive,
                OrgID = item.OrgID,
                OrgName = item.OrgName,
                DefClientAddressID = item.DefClientAddressID,
                DefBillAddressID = item.DefBillAddressID,
                DefShipAddressID = item.DefShipAddressID,
                NNINOrg = item.NNINOrg,
                PrimaryOrg = item.PrimaryOrg,
                OrgActive = item.OrgActive,
                OrgTypeID = item.OrgTypeID,
                OrgTypeName = item.OrgTypeName,
                ChargeTypeID = item.ChargeTypeID,
                ChargeTypeName = item.ChargeTypeName,
                ChargeTypeAccountID = item.ChargeTypeAccountID,
                MaxChargeTypeID = item.MaxChargeTypeID,
                MaxChargeTypeName = item.MaxChargeTypeName,
                EmailRank = item.EmailRank
            };
        }

        public static IEnumerable<ClientItem> GetClientItems(this IQueryable<ClientOrgInfoBase> query)
        {
            return query.Select(x => new ClientItem()
            {
                ClientID = x.ClientID,
                UserName = x.UserName,
                FName = x.FName,
                MName = x.MName,
                LName = x.LName,
                DemCitizenID = x.DemCitizenID,
                DemCitizenName = x.DemCitizenName,
                DemGenderID = x.DemGenderID,
                DemGenderName = x.DemGenderName,
                DemRaceID = x.DemRaceID,
                DemRaceName = x.DemRaceName,
                DemEthnicID = x.DemEthnicID,
                DemEthnicName = x.DemEthnicName,
                DemDisabilityID = x.DemDisabilityID,
                DemDisabilityName = x.DemDisabilityName,
                Privs = x.Privs,
                Communities = x.Communities,
                TechnicalInterestID = x.TechnicalInterestID,
                TechnicalInterestName = x.TechnicalInterestName,
                IsChecked = x.IsChecked,
                IsSafetyTest = x.IsSafetyTest,
                ClientActive = x.ClientActive,
                ClientOrgID = x.ClientOrgID,
                Phone = x.Phone,
                Email = x.Email,
                IsManager = x.IsManager,
                IsFinManager = x.IsFinManager,
                SubsidyStartDate = x.SubsidyStartDate,
                NewFacultyStartDate = x.NewFacultyStartDate,
                ClientAddressID = x.ClientAddressID,
                DepartmentID = x.DepartmentID,
                DepartmentName = x.DepartmentName,
                RoleID = x.RoleID,
                RoleName = x.RoleName,
                ClientOrgActive = x.ClientOrgActive,
                OrgID = x.OrgID,
                OrgName = x.OrgName,
                DefClientAddressID = x.DefClientAddressID,
                DefBillAddressID = x.DefBillAddressID,
                DefShipAddressID = x.DefShipAddressID,
                NNINOrg = x.NNINOrg,
                PrimaryOrg = x.PrimaryOrg,
                OrgActive = x.OrgActive,
                OrgTypeID = x.OrgTypeID,
                OrgTypeName = x.OrgTypeName,
                ChargeTypeID = x.ChargeTypeID,
                ChargeTypeName = x.ChargeTypeName,
                ChargeTypeAccountID = x.ChargeTypeAccountID,
                MaxChargeTypeID = x.MaxChargeTypeID,
                MaxChargeTypeName = x.MaxChargeTypeName,
                EmailRank = x.EmailRank
            });
        }

        public static MenuItem GetMenuItem(this Menu item)
        {
            if (item == null) return null;
            return item.Model<MenuItem>();
        }
    }
}
