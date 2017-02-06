using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LNF.Tests.Data
{
    [TestClass]
    public class OrgInfoTests
    {
        [TestMethod]
        public void OrgInfoTests_CanGetOrgInfo()
        {
            using (Providers.DataAccess.StartUnitOfWork())
            {
                var org1 = DA.Current.Single<OrgInfo>(17);
                AssertOrgInfo(true, org1);

                var alog1 = DA.Current.Single<ActiveLogOrg>(4346);
                AssertActiveLogOrg(true, alog1);

                var acct1 = DA.Current.Single<AccountInfo>(67);
                AssertAccountInfo(true, acct1);

                var alog2 = DA.Current.Single<ActiveLogAccount>(379);
                AssertActiveLogAccount(true, alog2);

                var c1 = DA.Current.Single<ClientInfo>(1301);
                AssertClientOrgInfo(true, c1);

                var alog3 = DA.Current.Single<ActiveLogClient>(10607);
                AssertActiveLogClient(true, alog3);

                var co1 = DA.Current.Single<ClientOrgInfo>(1373);
                AssertClientOrgInfo(true, co1);

                var alog4 = DA.Current.Single<ActiveLogClientOrg>(10608);
                AssertActiveLogClientOrg(true, alog4);

                var ca1 = DA.Current.Single<ClientAccountInfo>(3202);
                AssertClientAccountInfo(true, ca1);

                var alog5 = DA.Current.Single<ActiveLogClientAccount>(10610);
                AssertActiveLogClientAccount1(false, alog5);

                var alog6 = DA.Current.Single<ActiveLogClientAccount>(24078);
                AssertActiveLogClientAccount2(true, alog6);
            }
        }

        private void AssertOrgInfo(bool active, OrgInfoBase org)
        {
            Assert.AreEqual(17, org.OrgID);
            Assert.AreEqual("University of Michigan", org.OrgName);
            Assert.AreEqual(18, org.DefClientAddressID);
            Assert.AreEqual(0, org.DefBillAddressID);
            Assert.AreEqual(0, org.DefShipAddressID);
            Assert.AreEqual(true, org.NNINOrg);
            Assert.AreEqual(true, org.PrimaryOrg);
            Assert.AreEqual(true, org.OrgActive);
            Assert.AreEqual(1, org.OrgTypeID);
            Assert.AreEqual("U of Michigan (US)", org.OrgTypeName);
            Assert.AreEqual(5, org.ChargeTypeID);
            Assert.AreEqual("UMich", org.ChargeTypeName);
            Assert.AreEqual(67, org.ChargeTypeAccountID);
            AssertOrgInfoIsActive(active, org);
        }

        private void AssertActiveLogOrg(bool active, ActiveLogOrg alog)
        {
            Assert.AreEqual(4346, alog.LogID);
            Assert.AreEqual(17, alog.Record);
            Assert.AreEqual(DateTime.Parse("2002-08-30 00:00:00.000"), alog.EnableDate);
            Assert.AreEqual(null, alog.DisableDate);
            AssertOrgInfoIsActive(active, alog);
            AssertActiveLogItemIsActive(active, alog);
            AssertOrgInfo(active, alog);
        }

        private void AssertAccountItem(IAccountItem acct)
        {
            Assert.AreEqual(67, acct.AccountID);
            Assert.AreEqual("LNF General Lab Operating Account", acct.AccountName);
            Assert.AreEqual("61328052000216131RCHRG92320U023440", acct.Number);
            Assert.AreEqual("943777", acct.ShortCode);
            Assert.AreEqual(0, acct.BillAddressID);
            Assert.AreEqual(0, acct.ShipAddressID);
            Assert.AreEqual(string.Empty, acct.InvoiceNumber);
            Assert.AreEqual(null, acct.InvoiceLine1);
            Assert.AreEqual(null, acct.InvoiceLine2);
            Assert.AreEqual(null, acct.PoEndDate);
            Assert.AreEqual(null, acct.PoInitialFunds);
            Assert.AreEqual(null, acct.PoRemainingFunds);
            Assert.AreEqual("U023440", acct.Project);
            Assert.AreEqual(true, acct.AccountActive);
            Assert.AreEqual(8, acct.FundingSourceID);
            Assert.AreEqual("University Funds", acct.FundingSourceName);
            Assert.AreEqual(4, acct.TechnicalFieldID);
            Assert.AreEqual("MEMS/Mechanical", acct.TechnicalFieldName);
            Assert.AreEqual(1, acct.SpecialTopicID);
            Assert.AreEqual("None", acct.SpecialTopicName);
            Assert.AreEqual(3, acct.AccountTypeID);
            Assert.AreEqual("LimitedCharges", acct.AccountTypeName);
        }

        private void AssertAccountInfo(bool active, AccountInfoBase acct)
        {
            AssertOrgInfoIsActive(active, acct);
            AssertAccountItem(acct);
            AssertOrgInfo(active, acct);
        }

        private void AssertActiveLogAccount(bool active, ActiveLogAccount alog)
        {
            Assert.AreEqual(379, alog.LogID);
            Assert.AreEqual(67, alog.Record);
            Assert.AreEqual(DateTime.Parse("2002-08-30 00:00:00.000"), alog.EnableDate);
            Assert.AreEqual(null, alog.DisableDate);
            AssertOrgInfoIsActive(active, alog);
            AssertActiveLogItemIsActive(active, alog);
            AssertAccountInfo(active, alog);
        }

        private void AssertClientOrgInfo(bool active, ClientOrgInfoBase co)
        {
            Assert.AreEqual(1301, co.ClientID);
            Assert.AreEqual("jgett", co.UserName);
            Assert.AreEqual("James", co.FName);
            Assert.AreEqual(string.Empty, co.MName);
            Assert.AreEqual("Getty", co.LName);
            Assert.AreEqual("Getty, James", co.DisplayName);
            Assert.AreEqual((ClientPrivilege)3942, co.Privs);
            Assert.AreEqual(11, co.Communities);
            Assert.AreEqual(false, co.IsChecked);
            Assert.AreEqual(true, co.IsSafetyTest);
            Assert.AreEqual(true, co.ClientActive);
            Assert.AreEqual(1, co.DemCitizenID);
            Assert.AreEqual("No data", co.DemCitizenName);
            Assert.AreEqual(1, co.DemGenderID);
            Assert.AreEqual("No data", co.DemGenderName);
            Assert.AreEqual(1, co.DemRaceID);
            Assert.AreEqual("No data", co.DemRaceName);
            Assert.AreEqual(1, co.DemEthnicID);
            Assert.AreEqual("No data", co.DemEthnicName);
            Assert.AreEqual(1, co.DemDisabilityID);
            Assert.AreEqual("No data", co.DemDisabilityName);
            Assert.AreEqual(1, co.TechnicalInterestID);
            Assert.AreEqual("Electronics", co.TechnicalInterestName);
            Assert.AreEqual(1373, co.ClientOrgID);
            Assert.AreEqual("734-615-4108", co.Phone);
            Assert.AreEqual("jgett@umich.edu", co.Email);
            Assert.AreEqual(false, co.IsManager);
            Assert.AreEqual(false, co.IsFinManager);
            Assert.AreEqual(DateTime.Parse("2009-07-01 00:00:00.0000000"), co.SubsidyStartDate);
            Assert.AreEqual(DateTime.Parse("2007-01-01 00:00:00.0000000"), co.NewFacultyStartDate);
            Assert.AreEqual(1604, co.ClientAddressID);
            Assert.AreEqual(true, co.ClientOrgActive);
            Assert.AreEqual(17, co.DepartmentID);
            Assert.AreEqual("EECS", co.DepartmentName);
            Assert.AreEqual(6, co.RoleID);
            Assert.AreEqual("Other Support Staff", co.RoleName);
            Assert.AreEqual(5, co.MaxChargeTypeID);
            Assert.AreEqual("UMich", co.MaxChargeTypeName);
            Assert.AreEqual(1, co.EmailRank);
            AssertOrgInfoIsActive(active, co);
            AssertOrgInfo(active, co);
        }

        private void AssertActiveLogClient(bool active, ActiveLogClient alog)
        {
            Assert.AreEqual(10607, alog.LogID);
            Assert.AreEqual(1301, alog.Record);
            Assert.AreEqual(DateTime.Parse("2010-05-17 00:00:00.000"), alog.EnableDate);
            Assert.AreEqual(null, alog.DisableDate);
            AssertOrgInfoIsActive(active, alog);
            AssertActiveLogItemIsActive(active, alog);
            AssertClientOrgInfo(active, alog);
        }

        private void AssertActiveLogClientOrg(bool active, ActiveLogClientOrg alog)
        {
            Assert.AreEqual(10608, alog.LogID);
            Assert.AreEqual(1373, alog.Record);
            Assert.AreEqual(DateTime.Parse("2010-05-17 00:00:00.000"), alog.EnableDate);
            Assert.AreEqual(null, alog.DisableDate);
            AssertOrgInfoIsActive(active, alog);
            AssertActiveLogItemIsActive(active, alog);
            AssertClientOrgInfo(active, alog);
        }

        private void AssertClientAccountInfo(bool active, ClientAccountInfoBase ca)
        {
            Assert.AreEqual(3202, ca.ClientAccountID);
            Assert.AreEqual(true, ca.IsDefault);
            Assert.AreEqual(false, ca.Manager);
            Assert.AreEqual(true, ca.ClientAccountActive);
            AssertOrgInfoIsActive(active, ca);
            AssertAccountItem(ca);
            AssertClientOrgInfo(active, ca);
        }

        private void AssertActiveLogClientAccount1(bool active, ActiveLogClientAccount alog)
        {
            Assert.AreEqual(10610, alog.LogID);
            Assert.AreEqual(3202, alog.Record);
            Assert.AreEqual(DateTime.Parse("2010-05-17 00:00:00.000"), alog.EnableDate);
            Assert.AreEqual(DateTime.Parse("2015-11-05 00:00:00.000"), alog.DisableDate);
            AssertOrgInfoIsActive(active, alog);
            AssertActiveLogItemIsActive(active, alog);
            AssertClientAccountInfo(active, alog);
        }

        private void AssertActiveLogClientAccount2(bool active, ActiveLogClientAccount alog)
        {
            Assert.AreEqual(24078, alog.LogID);
            Assert.AreEqual(3202, alog.Record);
            Assert.AreEqual(DateTime.Parse("2015-11-04 00:00:00.000"), alog.EnableDate);
            Assert.AreEqual(null, alog.DisableDate);
            AssertOrgInfoIsActive(active, alog);
            AssertActiveLogItemIsActive(active, alog);
            AssertClientAccountInfo(active, alog);
        }

        private void AssertOrgInfoIsActive(bool active, OrgInfoBase item)
        {
            Assert.AreEqual(active, item.IsActive());
        }

        private void AssertActiveLogItemIsActive(bool active, IActiveLogItem item)
        {
            Assert.AreEqual(active, ActiveLogUtility.IsActive(item));
        }
    }
}
