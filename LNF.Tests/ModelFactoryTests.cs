using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class ModelFactoryTests : TestBase
    {
        [TestMethod]
        public void CanGetModels()
        {
            CanGetClientModel();
            CanGetClientAccountModel();
            CanGetOrgModel();
            CanGetAccountModel();
            CanGetGlobaCostModel();
            CanGetRoomModel();
            CanGetBuildingModel();
            CanGetLabModel();
            CanGetProcessTechModel();
            CanGetResourceModel();
            CanGetResourceClientModel();
            CanGetReservationModel();
        }

        //[TestMethod]
        public void CanGetClientModel()
        {
            IClient c;

            var client = DA.Current.Single<Client>(1301);
            c = client.CreateModel<IClient>();
            AssertClient(c);

            var clientInfo = DA.Current.Single<ClientInfo>(1301);
            c = clientInfo.CreateModel<IClient>();
            AssertClient(c);
            AssertObjectsAreSame(c, clientInfo);

            var clientOrg = DA.Current.Single<ClientOrg>(1373);
            c = clientOrg.CreateModel<IClient>();
            AssertClient(c);

            var clientOrgInfo = DA.Current.Single<ClientOrgInfo>(1373);
            c = clientOrgInfo.CreateModel<IClient>();
            AssertClient(c);
            AssertObjectsAreSame(c, clientOrgInfo);

            var clients = DA.Current.Query<Client>().Where(x => (x.Privs & ClientPrivilege.Staff) > 0 && x.Active);
            var models = clients.CreateModels();
            Assert.AreEqual(clients.Count(), models.Count());
        }

        //[TestMethod]
        public void CanGetClientAccountModel()
        {
            IClientAccount ca;

            var clientAccount = DA.Current.Single<ClientAccount>(3202);
            ca = clientAccount.CreateModel<IClientAccount>();
            AssertClientAccount(ca);

            var clientAccountInfo = DA.Current.Single<ClientAccountInfo>(3202);
            ca = clientAccountInfo.CreateModel<IClientAccount>();
            AssertClientAccount(ca);
            AssertObjectsAreSame(ca, clientAccountInfo);

            var clientAccounts = DA.Current.Query<ClientAccount>().Where(x => x.Manager && x.Active);
            var models = clientAccounts.CreateModels();
            Assert.AreEqual(clientAccounts.Count(), models.Count());
        }

        //[TestMethod]
        public void CanGetOrgModel()
        {
            IOrg o;

            var org = DA.Current.Single<Org>(17);
            o = org.CreateModel<IOrg>();
            AssertOrg(o);

            var orgInfo = DA.Current.Single<OrgInfo>(17);
            o = orgInfo.CreateModel<IOrg>();
            AssertOrg(o);
            AssertObjectsAreSame(o, orgInfo);

            var orgs = DA.Current.Query<Org>().Where(x => x.OrgType.OrgTypeName == "Non-UM University (US)" && x.Active);
            var models = orgs.CreateModels();
            Assert.AreEqual(orgs.Count(), models.Count());
        }

        //[TestMethod]
        public void CanGetAccountModel()
        {
            IAccount a;

            var account = DA.Current.Single<Account>(67);
            a = account.CreateModel<IAccount>();
            AssertAccount(a);

            var accountInfo = DA.Current.Single<AccountInfo>(67);
            a = accountInfo.CreateModel<IAccount>();
            AssertAccount(a);
            AssertObjectsAreSame(a, accountInfo);

            var accounts = DA.Current.Query<Account>().Where(x => x.AccountType.AccountTypeName == "Regular" && x.Active);
            var models = accounts.CreateModels();
            Assert.AreEqual(accounts.Count(), models.Count());
        }

        //[TestMethod]
        public void CanGetGlobaCostModel()
        {
            var globalCost = DA.Current.Single<GlobalCost>(8); // the only one
            var gc = globalCost.CreateModel<IGlobalCost>();
            Assert.AreEqual(8, gc.GlobalID);
            Assert.AreEqual(3, gc.BusinessDay);
            Assert.AreEqual(67, gc.LabAccountID);
            Assert.AreEqual(217, gc.LabCreditAccountID);
            Assert.AreEqual(714, gc.SubsidyCreditAccountID);
            Assert.AreEqual(439, gc.AdminID);
            Assert.AreEqual(365, gc.AccessToOld);
            Assert.AreEqual(DateTime.Parse("2007-10-05 08:56:54.730"), gc.EffDate);
        }

        //[TestMethod]
        public void CanGetRoomModel()
        {
            Room room;
            IRoom r;

            room = DA.Current.Single<Room>(6); // Clean Room
            r = room.CreateModel<IRoom>();
            Assert.AreEqual(6, r.RoomID);
            Assert.AreEqual(154, r.ParentID);
            Assert.AreEqual("Clean Room", r.RoomName);
            Assert.AreEqual("Clean Room", r.RoomDisplayName);
            Assert.AreEqual(true, r.PassbackRoom);
            Assert.AreEqual(true, r.Billable);
            Assert.AreEqual(false, r.ApportionDailyFee);
            Assert.AreEqual(true, r.ApportionEntryFee);
            Assert.AreEqual(true, r.Active);

            room = DA.Current.Single<Room>(25); // Wet Chemistry
            r = room.CreateModel<IRoom>();
            Assert.AreEqual(25, r.RoomID);
            Assert.AreEqual(154, r.ParentID);
            Assert.AreEqual("Wet Chemistry", r.RoomName);
            Assert.AreEqual("ROBIN", r.RoomDisplayName);
            Assert.AreEqual(true, r.PassbackRoom);
            Assert.AreEqual(true, r.Billable);
            Assert.AreEqual(false, r.ApportionDailyFee);
            Assert.AreEqual(true, r.ApportionEntryFee);
            Assert.AreEqual(true, r.Active);

            room = DA.Current.Single<Room>(154); // LNF
            r = room.CreateModel<IRoom>();
            Assert.AreEqual(154, r.RoomID);
            Assert.IsNull(r.ParentID);
            Assert.AreEqual("LNF", r.RoomName);
            Assert.AreEqual("LNF", r.RoomDisplayName);
            Assert.AreEqual(false, r.PassbackRoom);
            Assert.AreEqual(true, r.Billable);
            Assert.AreEqual(true, r.ApportionDailyFee);
            Assert.AreEqual(false, r.ApportionEntryFee);
            Assert.AreEqual(true, r.Active);
        }

        //[TestMethod]
        public void CanGetBuildingModel()
        {
            var bldg = DA.Current.Single<Building>(4); // the only one
            var b = bldg.CreateModel<IBuilding>();
            Assert.AreEqual(4, b.BuildingID);
            Assert.AreEqual("EECS", b.BuildingName);
            Assert.AreEqual("Electrical Engineering and Computer Science", b.BuildingDescription);
            Assert.AreEqual(true, b.BuildingIsActive);
        }

        //[TestMethod]
        public void CanGetLabModel()
        {
            var lab = DA.Current.Single<Lab>(9); // Wet Chemistry
            var l = lab.CreateModel<ILab>();
            Assert.AreEqual(9, l.LabID);
            Assert.AreEqual(4, l.BuildingID);
            Assert.AreEqual("Wet Chemistry", l.LabName);
            Assert.AreEqual("ROBIN", l.LabDisplayName);
            Assert.AreEqual("Room 1436 EECS", l.LabDescription);
            Assert.AreEqual(25, l.RoomID);
            Assert.AreEqual(true, l.LabIsActive);
            Assert.AreEqual(true, l.BuildingIsActive);
            Assert.AreEqual("EECS", l.BuildingName);
            Assert.AreEqual("Electrical Engineering and Computer Science", l.BuildingDescription);
            Assert.AreEqual("ROBIN", l.RoomDisplayName);
            Assert.AreEqual("Wet Chemistry", l.RoomName);
        }

        //[TestMethod]
        public void CanGetProcessTechModel()
        {
            var procTech = DA.Current.Single<ProcessTech>(28); // Annealing (Wet Chemistry)
            var pt = procTech.CreateModel<IProcessTech>();
            Assert.AreEqual(28, pt.ProcessTechID);
            Assert.AreEqual(24, pt.ProcessTechGroupID);
            Assert.AreEqual("Annealing", pt.ProcessTechGroupName);
            Assert.AreEqual("Metrology", pt.ProcessTechDescription);
            Assert.AreEqual(true, pt.ProcessTechIsActive);
            Assert.AreEqual("Annealing", pt.ProcessTechName);
            Assert.AreEqual("Room 1436 EECS", pt.LabDescription);
            Assert.AreEqual("ROBIN", pt.LabDisplayName);
            Assert.AreEqual(9, pt.LabID);
            Assert.AreEqual(true, pt.LabIsActive);
            Assert.AreEqual("Wet Chemistry", pt.LabName);
            Assert.AreEqual("ROBIN", pt.RoomDisplayName);
            Assert.AreEqual(25, pt.RoomID);
            Assert.AreEqual("Wet Chemistry", pt.RoomName);
            Assert.AreEqual("Electrical Engineering and Computer Science", pt.BuildingDescription);
            Assert.AreEqual(4, pt.BuildingID);
            Assert.AreEqual(true, pt.BuildingIsActive);
            Assert.AreEqual("EECS", pt.BuildingName);
        }

        //[TestMethod]
        public void CanGetResourceModel()
        {
            IResource r;

            var resource = DA.Current.Single<Resource>(10020);
            r = resource.CreateModel<IResource>();
            AssertResource(r);

            var resourceInfo = DA.Current.Single<ResourceInfo>(10020);
            r = resourceInfo.CreateModel<IResource>();
            AssertResource(r);
            AssertObjectsAreSame(r, resourceInfo);

            var resources = DA.Current.Query<Resource>().Where(x => x.ProcessTech.ProcessTechID == 6 && x.IsActive);
            var models = resources.CreateModels();
            Assert.AreEqual(resources.Count(), models.Count());
        }

        //[TestMethod]
        public void CanGetResourceClientModel()
        {
            IResourceClient rc;

            var resourceClient = DA.Current.Single<ResourceClient>(22124);
            rc = resourceClient.CreateModel<IResourceClient>();
            AssertResourceClient(rc);

            var resourceClientInfo = DA.Current.Single<ResourceClientInfo>(22124);
            rc = resourceClientInfo.CreateModel<IResourceClient>();
            AssertResourceClient(rc);
            AssertObjectsAreSame(rc, resourceClientInfo);

            var resourceClients = DA.Current.Query<ResourceClient>().Where(x => x.AuthLevel == ClientAuthLevel.ToolEngineer);
            var models = resourceClients.CreateModels();
            Assert.AreEqual(resourceClients.Count(), models.Count());
        }

        //[TestMethod]
        public void CanGetReservationModel()
        {
            IReservation rsv;

            var reservation = DA.Current.Single<Reservation>(123456);
            rsv = reservation.CreateModel<IReservation>();
            AssertReservation(rsv);

            var reservationInfo = DA.Current.Single<ReservationInfo>(123456);
            rsv = reservationInfo.CreateModel<IReservation>();
            AssertReservation(rsv);
            AssertObjectsAreSame(rsv, reservationInfo);

            var reservations = DA.Current.Query<Reservation>().Where(x => x.Resource.ResourceID == 62040 && x.BeginDateTime < DateTime.Parse("2008-04-29") && x.EndDateTime > DateTime.Parse("2008-04-28"));
            var models = reservations.CreateModels();
            var comparer = new ReservationComparer();
            AssertCollectionsAreSame(models, reservations, (x, y) => x.ReservationID == y.ReservationID, comparer);
        }

        private void AssertOrg(IOrg o)
        {
            Assert.AreEqual(17, o.OrgID);
            Assert.AreEqual("University of Michigan", o.OrgName);
            Assert.AreEqual(18, o.DefClientAddressID);
            Assert.AreEqual(0, o.DefBillAddressID);
            Assert.AreEqual(0, o.DefShipAddressID);
            Assert.AreEqual(true, o.NNINOrg);
            Assert.AreEqual(true, o.PrimaryOrg);
            Assert.AreEqual(true, o.OrgActive);
            Assert.AreEqual(1, o.OrgTypeID);
            Assert.AreEqual("U of Michigan (US)", o.OrgTypeName);
            Assert.AreEqual(5, o.ChargeTypeID);
            Assert.AreEqual("UMich", o.ChargeTypeName);
            Assert.AreEqual(67, o.ChargeTypeAccountID);
        }

        private void AssertAccount(IAccount a)
        {
            Assert.AreEqual(67, a.AccountID);
            Assert.AreEqual("LNF General Lab Operating Account", a.AccountName);
            Assert.AreEqual("61328052000216131RCHRG92320U023440", a.AccountNumber);
            Assert.AreEqual("943777", a.ShortCode);
            Assert.AreEqual(0, a.BillAddressID);
            Assert.AreEqual(0, a.ShipAddressID);
            Assert.AreEqual("", a.InvoiceNumber);
            Assert.AreEqual(null, a.InvoiceLine1);
            Assert.AreEqual(null, a.InvoiceLine2);
            Assert.AreEqual(null, a.PoEndDate);
            Assert.AreEqual(null, a.PoInitialFunds);
            Assert.AreEqual(null, a.PoRemainingFunds);
            Assert.AreEqual("U023440", a.Project);
            Assert.AreEqual(true, a.AccountActive);
            Assert.AreEqual(8, a.FundingSourceID);
            Assert.AreEqual("University Funds", a.FundingSourceName);
            Assert.AreEqual(4, a.TechnicalFieldID);
            Assert.AreEqual("MEMS/Mechanical", a.TechnicalFieldName);
            Assert.AreEqual(1, a.SpecialTopicID);
            Assert.AreEqual("None", a.SpecialTopicName);
            Assert.AreEqual(3, a.AccountTypeID);
            Assert.AreEqual("LimitedCharges", a.AccountTypeName);
            Assert.AreEqual("[943777] LNF General Lab Operating Account", a.NameWithShortCode);
            Assert.AreEqual("[943777] LNF General Lab Operating Account (University of Michigan)", a.FullAccountName);
            Assert.AreEqual(false, a.IsRegularAccountType);
            AssertOrg(a);
        }

        private void AssertClient(IClient c)
        {
            Assert.AreEqual(true, c.ClientActive);
            Assert.AreEqual(1301, c.ClientID);
            Assert.AreEqual("jgett", c.UserName);
            Assert.AreEqual("James", c.FName);
            Assert.AreEqual("", c.MName);
            Assert.AreEqual("Getty", c.LName);
            Assert.AreEqual("Getty, James", c.DisplayName);
            Assert.AreEqual((ClientPrivilege)3942, c.Privs);
            Assert.AreEqual(11, c.Communities);
            Assert.AreEqual(false, c.IsChecked);
            Assert.AreEqual(true, c.IsSafetyTest);
            Assert.AreEqual(true, c.ClientActive);
            Assert.AreEqual(1, c.DemCitizenID);
            Assert.AreEqual("No data", c.DemCitizenName);
            Assert.AreEqual(1, c.DemGenderID);
            Assert.AreEqual("No data", c.DemGenderName);
            Assert.AreEqual(1, c.DemRaceID);
            Assert.AreEqual("No data", c.DemRaceName);
            Assert.AreEqual(1, c.DemEthnicID);
            Assert.AreEqual("No data", c.DemEthnicName);
            Assert.AreEqual(1, c.DemDisabilityID);
            Assert.AreEqual("No data", c.DemDisabilityName);
            Assert.AreEqual(1, c.TechnicalInterestID);
            Assert.AreEqual("Electronics", c.TechnicalInterestName);
            Assert.AreEqual(1373, c.ClientOrgID);
            Assert.AreEqual("734-615-4108", c.Phone);
            Assert.AreEqual("jgett@umich.edu", c.Email);
            Assert.AreEqual(false, c.IsManager);
            Assert.AreEqual(false, c.IsFinManager);
            Assert.AreEqual(DateTime.Parse("2009-07-01 00:00:00"), c.SubsidyStartDate);
            Assert.AreEqual(DateTime.Parse("2007-01-01 00:00:00"), c.NewFacultyStartDate);
            Assert.AreEqual(1604, c.ClientAddressID);
            Assert.AreEqual(true, c.ClientOrgActive);
            Assert.AreEqual(17, c.DepartmentID);
            Assert.AreEqual("EECS", c.DepartmentName);
            Assert.AreEqual(6, c.RoleID);
            Assert.AreEqual("Other Support Staff", c.RoleName);
            Assert.AreEqual(5, c.MaxChargeTypeID);
            Assert.AreEqual("UMich", c.MaxChargeTypeName);
            Assert.AreEqual(1, c.EmailRank);
            AssertOrg(c);
        }

        private void AssertClientAccount(IClientAccount ca)
        {
            Assert.AreEqual(true, ca.ClientAccountActive);
            Assert.AreEqual(3202, ca.ClientAccountID);
            Assert.AreEqual(true, ca.IsDefault);
            Assert.AreEqual(false, ca.Manager);
            Assert.AreEqual(true, ca.ClientAccountActive);
            AssertClient(ca);
            AssertAccount(ca);
        }

        private void AssertResource(IResource r)
        {
            Assert.AreEqual(10020, r.ResourceID);
            Assert.AreEqual("LAM 9400", r.ResourceName);
            Assert.AreEqual(true, r.ResourceIsActive);
            Assert.AreEqual(true, r.IsSchedulable);
            Assert.AreEqual("A low pressure, high density plasma etcher used for dry etching sub micron features in the polysilicon process\r\n\r\nTool specifications:\r\nMaximum substrate size is 6-inch\r\nSmaller substrates must be mounted on a 6-inch carrier\r\nProcess Gas: HBr/Cl2/SF6/He/O2/Ar/10% O2 in He", r.ResourceDescription);
            Assert.AreEqual("helpdesk.patterning@lnf.umich.edu", r.HelpdeskEmail);
            Assert.AreEqual("http://lnf-wiki.eecs.umich.edu/wiki/LAM_9400", r.WikiPageUrl);
            Assert.AreEqual(ResourceState.Online, r.State);
            Assert.AreEqual("", r.StateNotes);
            Assert.AreEqual(12, r.AuthDuration);
            Assert.AreEqual(true, r.AuthState);
            Assert.AreEqual(10080, r.ReservFence);
            Assert.AreEqual(960, r.MaxAlloc);
            Assert.AreEqual(0, r.MinCancelTime);
            Assert.AreEqual(10, r.ResourceAutoEnd);
            Assert.AreEqual(null, r.UnloadTime);
            Assert.AreEqual(15, r.Granularity);
            Assert.AreEqual(0, r.Offset);
            Assert.AreEqual(true, r.IsReady);
            Assert.AreEqual(15, r.MinReservTime);
            Assert.AreEqual(360, r.MaxReservTime);
            Assert.AreEqual(15, r.GracePeriod);
            Assert.AreEqual("LAM 9400 [10020]", r.ResourceDisplayName);
            Assert.AreEqual(6, r.ProcessTechID);
            Assert.AreEqual(6, r.ProcessTechGroupID);
            Assert.AreEqual("Plasma Etch", r.ProcessTechGroupName);
            Assert.AreEqual("Plasma Etch", r.ProcessTechName);
            Assert.AreEqual("Plasma Etch", r.ProcessTechDescription);
            Assert.AreEqual(true, r.ProcessTechIsActive);
            Assert.AreEqual(1, r.LabID);
            Assert.AreEqual("Clean Room", r.LabName);
            Assert.AreEqual("Clean Room", r.LabDisplayName);
            Assert.AreEqual("Solid-State Lab", r.LabDescription);
            Assert.AreEqual(6, r.RoomID);
            Assert.AreEqual("Clean Room", r.RoomName);
            Assert.AreEqual("Clean Room", r.RoomDisplayName);
            Assert.AreEqual(true, r.LabIsActive);
            Assert.AreEqual(4, r.BuildingID);
            Assert.AreEqual("EECS", r.BuildingName);
            Assert.AreEqual("Electrical Engineering and Computer Science", r.BuildingDescription);
            Assert.AreEqual(true, r.BuildingIsActive);
        }

        private void AssertResourceClient(IResourceClient rc)
        {
            Assert.AreEqual(22124, rc.ResourceClientID);
            Assert.AreEqual(54040, rc.ResourceID);
            Assert.AreEqual(1301, ((IPrivileged)rc).ClientID);
            Assert.AreEqual("jgett", rc.UserName);
            Assert.AreEqual((ClientPrivilege)3942, rc.Privs);
            Assert.AreEqual((ClientAuthLevel)8, rc.AuthLevel);
            Assert.AreEqual(null, rc.Expiration);
            Assert.AreEqual(0, rc.EmailNotify);
            Assert.AreEqual(null, rc.PracticeResEmailNotify);
            Assert.AreEqual("ACS 200 cluster tool", rc.ResourceName);
            Assert.AreEqual(12, rc.AuthDuration);
            Assert.AreEqual("Getty, James", rc.DisplayName);
            Assert.AreEqual("jgett@umich.edu", rc.Email);
            Assert.AreEqual(true, rc.ClientActive);
            Assert.AreEqual(true, rc.ResourceIsActive);

        }

        private void AssertReservation(IReservation rsv)
        {
            Assert.AreEqual(123456, rsv.ReservationID);
            Assert.AreEqual(62040, rsv.ResourceID);
            Assert.AreEqual("SJ-20 Evaporator", rsv.ResourceName);
            Assert.AreEqual("Used for metal deposition.\r\nFixturing: Lift-off dome.\r\nTool specifications:\r\nSubstrate size:Fragments up to 4 inch.\r\nMaximum capacity: 9, 4\" wafers.\r\nMaterial maximum thickness: \r\nGe\t   4000Ǻ\r\nAu\t   4000Ǻ\r\nNi         2000Ǻ\r\nTi         8000Ǻ", rsv.ResourceDescription);
            Assert.AreEqual(15, rsv.Granularity);
            Assert.AreEqual(10080, rsv.ReservFence);
            Assert.AreEqual(90, rsv.MinReservTime);
            Assert.AreEqual(240, rsv.MaxReservTime);
            Assert.AreEqual(1560, rsv.MaxAlloc);
            Assert.AreEqual(0, rsv.Offset);
            Assert.AreEqual(15, rsv.GracePeriod);
            Assert.AreEqual(30, rsv.ResourceAutoEnd);
            Assert.AreEqual(0, rsv.MinCancelTime);
            Assert.AreEqual(15, rsv.UnloadTime);
            Assert.AreEqual(null, rsv.OTFSchedTime);
            Assert.AreEqual(true, rsv.AuthState);
            Assert.AreEqual(12, rsv.AuthDuration);
            Assert.AreEqual((ResourceState)2, rsv.State);
            Assert.AreEqual(true, rsv.IsSchedulable);
            Assert.AreEqual(true, rsv.ResourceIsActive);
            Assert.AreEqual("helpdesk.deposition@lnf.umich.edu", rsv.HelpdeskEmail);
            Assert.AreEqual(8, rsv.ProcessTechID);
            Assert.AreEqual("PVD", rsv.ProcessTechName);
            Assert.AreEqual(1, rsv.LabID);
            Assert.AreEqual("Clean Room", rsv.LabName);
            Assert.AreEqual("Clean Room", rsv.LabDisplayName);
            Assert.AreEqual(4, rsv.BuildingID);
            Assert.AreEqual("EECS", rsv.BuildingName);
            Assert.AreEqual(322, rsv.ClientID);
            Assert.AreEqual("junyang", rsv.UserName);
            Assert.AreEqual("Yang", rsv.LName);
            Assert.AreEqual("Jun", rsv.FName);
            Assert.AreEqual((ClientPrivilege)1029, rsv.Privs);
            Assert.AreEqual(424, rsv.AccountID);
            Assert.AreEqual("Quantum Dot Heterostructures for Slow Light Studie", rsv.AccountName);
            Assert.AreEqual("055991", rsv.ShortCode);
            Assert.AreEqual("764-3305", rsv.Phone);
            Assert.AreEqual("junyang@eecs.umich.edu", rsv.Email);
            Assert.AreEqual(6, rsv.ActivityID);
            Assert.AreEqual("Processing", rsv.ActivityName);
            Assert.AreEqual((ActivityAccountType)0, rsv.ActivityAccountType);
            Assert.AreEqual((ClientAuthLevel)30, rsv.StartEndAuth);
            Assert.AreEqual(true, rsv.Editable);
            Assert.AreEqual(false, rsv.IsRepair);
            Assert.AreEqual(false, rsv.IsFacilityDownTime);
            Assert.AreEqual(DateTime.Parse("2008-04-28 16:30:00.0000000"), rsv.BeginDateTime);
            Assert.AreEqual(DateTime.Parse("2008-04-28 16:45:00.0000000"), rsv.EndDateTime);
            Assert.AreEqual(DateTime.Parse("2008-04-28 16:30:00.0000000"), rsv.ActualBeginDateTime);
            Assert.AreEqual(DateTime.Parse("2008-04-28 16:45:00.0000000"), rsv.ActualEndDateTime);
            Assert.AreEqual(DateTime.Parse("2008-04-28 16:30:00.0000000"), rsv.ChargeBeginDateTime);
            Assert.AreEqual(DateTime.Parse("2008-04-28 16:45:00.0000000"), rsv.ChargeEndDateTime);
            Assert.AreEqual(-1, rsv.ClientIDBegin);
            Assert.AreEqual(null, rsv.ClientBeginLName);
            Assert.AreEqual(null, rsv.ClientBeginFName);
            Assert.AreEqual(-1, rsv.ClientIDEnd);
            Assert.AreEqual(null, rsv.ClientEndLName);
            Assert.AreEqual(null, rsv.ClientEndFName);
            Assert.AreEqual(DateTime.Parse("2008-04-28 11:52:49.9630000"), rsv.CreatedOn);
            Assert.AreEqual(DateTime.Parse("2008-04-28 11:52:49.9630000"), rsv.LastModifiedOn);
            Assert.AreEqual(120, rsv.Duration);
            Assert.AreEqual("", rsv.Notes);
            Assert.AreEqual(1, rsv.ChargeMultiplier);
            Assert.AreEqual(true, rsv.ApplyLateChargePenalty);
            Assert.AreEqual(false, rsv.AutoEnd);
            Assert.AreEqual(true, rsv.HasProcessInfo);
            Assert.AreEqual(false, rsv.HasInvitees);
            Assert.AreEqual(true, rsv.IsActive);
            Assert.AreEqual(false, rsv.IsStarted);
            Assert.AreEqual(false, rsv.IsUnloaded);
            Assert.AreEqual(null, rsv.RecurrenceID);
            Assert.AreEqual(null, rsv.GroupID);
            Assert.AreEqual(120, rsv.MaxReservedDuration);
            Assert.AreEqual(null, rsv.CancelledDateTime);
            Assert.AreEqual(false, rsv.KeepAlive);
            Assert.AreEqual(null, rsv.OriginalBeginDateTime);
            Assert.AreEqual(null, rsv.OriginalEndDateTime);
            Assert.AreEqual(null, rsv.OriginalModifiedOn);
            Assert.AreEqual(true, rsv.IsCurrentlyOutsideGracePeriod);
        }
    }

    public class ReservationComparer : Comparer<IReservation, Reservation>
    {
        public override bool AreEqual(IReservation obj1, Reservation obj2)
        {
            return obj1.UserName == obj2.Client.UserName
                && obj1.ReservationID == obj2.ReservationID;
        }
    }
}
