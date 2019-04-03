using LNF.Models.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Data
{
    public static class Extensions
    {
        public static ClientItem CreateClientItem(this ClientOrgInfoBase item)
        {
            if (item == null) return null;
            var list = new List<ClientOrgInfoBase> { item };
            return CreateClientItems(list.AsQueryable()).FirstOrDefault();
        }

        public static IEnumerable<ClientItem> CreateClientItems(this IQueryable<ClientOrgInfoBase> query)
        {
            if (query == null) return null;

            return query.Select(x => new ClientItem()
            {
                ClientID = x.ClientID,
                UserName = x.UserName,
                FName = x.FName,
                MName = x.MName,
                LName = x.LName,
                DemCitizenID = x.DemCitizenID,
                DemGenderID = x.DemGenderID,
                DemRaceID = x.DemRaceID,
                DemEthnicID = x.DemEthnicID,
                DemDisabilityID = x.DemDisabilityID,
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
            }).ToList();
        }

        public static MenuItem GetMenuItem(this Menu item)
        {
            if (item == null) return null;
            return item.CreateModel<MenuItem>();
        }

        public static GlobalSettingsItem CreateGlobalSettingsItem(this GlobalSettings item)
        {
            if (item == null) return null;
            var list = new List<GlobalSettings> { item };
            return CreateGlobalSettingsItems(list.AsQueryable()).FirstOrDefault();
        }

        public static IEnumerable<GlobalSettingsItem> CreateGlobalSettingsItems(this IQueryable<GlobalSettings> query)
        {
            if (query == null) return null;

            return query.Select(x => new GlobalSettingsItem
            {
                SettingID = x.SettingID,
                SettingName = x.SettingName,
                SettingValue = x.SettingValue
            }).ToList();
        }
    }
}
