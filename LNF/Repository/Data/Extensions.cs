using LNF.Models.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Data
{
    public static class Extensions
    {
        public static ClientAccountItem CreateClientAccountItem(this ClientAccountInfo item)
        {
            if (item == null) return null;
            var list = new List<ClientAccountInfo> { item };
            return CreateClientAccountItems(list.AsQueryable()).FirstOrDefault();
        }

        public static IEnumerable<ClientAccountItem> CreateClientAccountItems(this IQueryable<ClientAccountInfo> query)
        {
            if (query == null) return null;

            return query.Select(x => new ClientAccountItem
            {
                ClientAccountID = x.ClientAccountID,
                IsDefault = x.IsDefault,
                Manager = x.Manager,
                ClientAccountActive = x.ClientAccountActive,
                AccountID = x.AccountID,
                AccountName = x.AccountName,
                Number = x.Number,
                ShortCode = x.ShortCode,
                BillAddressID = x.BillAddressID,
                ShipAddressID = x.ShipAddressID,
                InvoiceNumber = x.InvoiceNumber,
                InvoiceLine1 = x.InvoiceLine1,
                InvoiceLine2 = x.InvoiceLine2,
                PoEndDate = x.PoEndDate,
                PoInitialFunds = x.PoInitialFunds,
                PoRemainingFunds = x.PoRemainingFunds,
                Project = x.Project,
                AccountActive = x.AccountActive,
                FundingSourceID = x.FundingSourceID,
                FundingSourceName = x.FundingSourceName,
                TechnicalFieldID = x.TechnicalFieldID,
                TechnicalFieldName = x.TechnicalFieldName,
                SpecialTopicID = x.SpecialTopicID,
                SpecialTopicName = x.SpecialTopicName,
                AccountTypeID = x.AccountTypeID,
                AccountTypeName = x.AccountTypeName,
                ClientOrgID = x.ClientOrgID,
                Phone = x.Phone,
                Email = x.Email,
                IsManager = x.IsManager,
                ClientOrgActive = x.ClientOrgActive,
                OrgID = x.OrgID,
                OrgName = x.OrgName,
                OrgActive = x.OrgActive,
                ClientID = x.ClientID,
                UserName = x.UserName,
                LName = x.LName,
                MName = x.MName,
                FName = x.FName,
                DisplayName = x.DisplayName,
                Privs = x.Privs,
                ClientActive = x.ClientActive
            }).ToList();
        }

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
            return item.Model<MenuItem>();
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
