using LNF.Billing;
using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.Data;
using LNF.Models.Billing;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class BillingTypeManager : ManagerBase, IBillingTypeManager
    {
        private IList<BillingType> _allBillingTypes;

        public BillingTypeManager(IProvider serviceProvider) : base(serviceProvider) { }

        public string GetBillingTypeName(IBillingType billingType)
        {
            if (billingType == ExtAc_Ga)
                return "External Academic GaAs";
            else if (billingType == ExtAc_Hour)
                return "External Academic Hour";
            else if (billingType == ExtAc_Si)
                return "External Academic Si";
            else if (billingType == ExtAc_Tools)
                return "External Academic Tools";
            else if (billingType == Int_Ga)
                return "Internal Academic GaAs";
            else if (billingType == Int_Hour)
                return "Internal Academic Hour";
            else if (billingType == Int_Si)
                return "Internal Academic Si";
            else if (billingType == Int_Tools)
                return "Internal Academic Tools";
            else if (billingType == NonAc)
                return "Non Academic";
            else if (billingType == NonAc_Hour)
                return "Non Academic Hour";
            else
                return "Other";
        }

        public IBillingType GetBillingType(string text)
        {
            switch (text)
            {
                case "External Academic GaAs":
                    return ExtAc_Ga;
                case "External Academic Hour":
                    return ExtAc_Hour;
                case "External Academic Si":
                    return ExtAc_Si;
                case "External Academic Tools":
                    return ExtAc_Tools;
                case "Internal Academic GaAs":
                    return Int_Ga;
                case "Internal Academic Hour":
                    return Int_Hour;
                case "Internal Academic Si":
                    return Int_Si;
                case "Internal Academic Tools":
                    return Int_Tools;
                case "Non Academic":
                    return NonAc;
                case "Non Academic Hour":
                    return NonAc_Hour;
                default:
                    return Other;
            }
        }

        private IBillingType GetBillingType(Client client, Account account, IEnumerable<ClientOrg> clientOrgs, IEnumerable<ClientAccount> clientAccounts, IEnumerable<ClientRemote> clientRemotes, IEnumerable<ClientOrgBillingTypeLog> cobtLogs)
        {
            //assume that the collections passed have already been filtered for start and end dates

            int record = 0;
            IBillingType result = null;

            ClientOrg co = clientOrgs.FirstOrDefault(x => x.Client == client && x.Org == account.Org);

            if (co != null)
            {
                //is null for remote runs
                ClientAccount ca = clientAccounts.FirstOrDefault(x => x.ClientOrg == co && x.Account == account);
                if (ca != null)
                    record = ca.ClientAccountID;
            }

            if (record == 0)
            {
                ClientRemote cr = clientRemotes.FirstOrDefault(x => x.Client == client && x.Account == account);
                if (cr != null)
                    record = cr.ClientRemoteID;
                if (record == 0)
                    result = RegularException;
                else
                    result = Remote;
            }
            else
            {
                ClientOrgBillingTypeLog cobtlog = cobtLogs.FirstOrDefault(x => x.ClientOrg == co);
                if (cobtlog != null)
                    result = cobtlog.BillingType.CreateModel<IBillingType>();
                if (result == null)
                    result = Regular;
            }

            return result;
        }

        public IBillingType GetBillingType(int clientId, int accountId, DateTime period)
        {
            // always add one more month for @Period, because we allow changes made during the current month that will take effect
            // as long as it's before the 4th business day of business
            // 2011-01-26 the above statement is not quite right.  We should not allow change after the Period.  if a change is made on 2011-01-04, it has nothing
            // to do with period = 2010-12-01
            //set @Period = dbo.udf_BusinessDate (DATEADD(MONTH, 1, @Period), null)

            DateTime sd = period;
            DateTime ed = sd.AddMonths(1);

            var client = Session.Single<Client>(clientId);
            var account = Session.Single<Account>(accountId);

            return GetBillingType(
                client,
                account,
                Session.Query<ClientOrg>().Where(x => x.Client == client && x.Org == account.Org).FindActive(x => x.ClientOrgID, sd, ed),
                Session.Query<ClientAccount>().Where(x => x.ClientOrg.Client == client && x.Account == account).FindActive(x => x.ClientAccountID, sd, ed),
                Session.Query<ClientRemote>().Where(x => x.Client == client && x.Account == account).FindActive(x => x.ClientRemoteID, sd, ed),
                ClientOrgBillingTypeLogUtility.GetActive(sd, ed).Where(x => x.ClientOrg.Client == client && x.ClientOrg.Org == account.Org).ToArray()
            );
        }

        public void Update(int clientId, DateTime period)
        {
            bool temp = Utility.IsCurrentPeriod(period);
            var step1 = new BillingDataProcessStep1(DateTime.Now, Provider);
            step1.PopulateToolBilling(period, clientId, temp);
            step1.PopulateRoomBilling(period, clientId, temp);
        }

        public IEnumerable<Models.Billing.IToolBilling> SelectToolBillingData(int clientId, DateTime period, bool temp)
        {
            string sql = "EXEC Billing.dbo.ToolData_Select @Action='ForToolBilling', @Period=:period, @ClientID=:ClientID";

            ISqlQuery query = Session.SqlQuery(sql).SetParameter("period", period).SetParameter("ClientID", clientId);

            IEnumerable<Models.Billing.IToolBilling> result;

            if (temp)
                result = query.List<ToolBillingTemp>().CreateModels<Models.Billing.IToolBilling>();
            else
                result = query.List<ToolBilling>().CreateModels<Models.Billing.IToolBilling>();

            return result;
        }

        public bool IsMonthlyUserBillingType(int billingTypeId)
        {
            // BillingType.Int_Ga || BillingType.Int_Si || BillingType.ExtAc_Ga || BillingType.ExtAc_Si
            bool result = new[] { Int_Ga.BillingTypeID, Int_Si.BillingTypeID, ExtAc_Ga.BillingTypeID, ExtAc_Si.BillingTypeID }.Contains(billingTypeId);
            return result;
        }

        public bool IsGrowerUserBillingType(int billingTypeId)
        {
            // BillingType.Int_Tools || BillingType.ExtAc_Tools || BillingType.NonAc_Tools
            bool result = new[] { Int_Tools.BillingTypeID, ExtAc_Tools.BillingTypeID, NonAc_Tools.BillingTypeID }.Contains(billingTypeId);
            return result;
        }

        public IBillingType Find(int billingTypeId)
        {
            return GetAllBillingTypes().FirstOrDefault(x => x.BillingTypeID == billingTypeId);
        }

        public IBillingType Find(string name)
        {
            return GetAllBillingTypes().FirstOrDefault(x => x.BillingTypeName == name);
        }

        public IBillingType GetBillingTypeByClientAndOrg(DateTime period, int clientId, int orgId, IEnumerable<IHoliday> holidays)
        {
            // always add one more month for period, because we allow changes made during the current month that will take effect
            // as long as it's before the 4th business day of business
            DateTime p = Utility.NextBusinessDay(period.AddMonths(1), holidays);

            var cobtLog = Session.Query<ClientOrgBillingTypeLog>().FirstOrDefault(x => x.ClientOrg.Client.ClientID == clientId && x.ClientOrg.Org.OrgID == orgId && x.EffDate < p && (x.DisableDate == null || x.DisableDate > p));

            if (cobtLog != null)
                return cobtLog.BillingType.CreateModel<IBillingType>();
            else
                return Default;
        }

        /// <summary>
        /// The final billed amount based on BillingType and Tool.
        /// </summary>
        public decimal GetLineCost(Models.Billing.IToolBilling item)
        {
            // [2015-11-13 jg] this is identical to the logic originally in:
            //      1) sselFinOps.AppCode.BLL.FormulaBL.ApplyToolFormula (for External Invoice)
            //      2) sselIndReports.AppCode.Bll.ToolBillingBL.GetToolBillingDataByClientID20110701 (for User Usage Summary)
            //      3) LNF.WebApi.Billing.Models.ReportUtility.ApplyToolFormula (for SUB reports, note: this is the replacement for the Billing WCF service)
            //
            //      I think at this point all the formulas can be replaced by GetTotalCharge()
            //      because each value used by the formula should correctly reflect the rules
            //      in place during the given period (or at least that is the goal).

            decimal result = 0;

            //if rates are 0 everything must be 0 (this was at the end, but why not do it at the beginning?)
            if (item.ResourceRate + item.PerUseRate == 0)
                result = 0;

            int cleanRoomId = 6;
            int maskMakerId = 56000;

            if (IsMonthlyUserBillingType(item.BillingTypeID)) //not used at this point but maybe in the future
            {
                // Monthly User, charge mask maker for everyone
                if (item.RoomID == cleanRoomId) //Clean Room
                {
                    if (item.ResourceID == maskMakerId) //Mask Maker
                    {
                        if (item.IsStarted)
                            result = item.UsageFeeCharged + item.OverTimePenaltyFee + (item.ResourceRate == 0 ? 0 : item.ReservationFee2);
                        else
                            result = item.UncancelledPenaltyFee + item.ReservationFee2;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else
                {
                    //non clean room tools are always charged for usage fee
                    if (item.IsStarted)
                        result = item.UsageFeeCharged + item.OverTimePenaltyFee + (item.ResourceRate == 0 ? 0 : item.ReservationFee2);
                    else
                        result = item.UncancelledPenaltyFee + item.ReservationFee2;
                }
            }
            else if (item.BillingTypeID == BillingTypeItem.Other)
            {
                //based on sselIndReports.AppCode.BLL.ToolBillingBL.GetToolBillingDataByClientID20110701 the Other billing type is not set to zero any longer
                result = item.GetTotalCharge();
            }
            else
            {
                //Per Use types
                if (item.Period >= new DateTime(2010, 7, 1))
                {
                    //2011-05 New tool billing started on 2011-04
                    if (item.Period >= new DateTime(2011, 4, 1))
                    {
                        if (!item.IsCancelledBeforeAllowedTime)
                            result = item.UsageFeeCharged + item.OverTimePenaltyFee + item.BookingFee; //should be the same as GetTotalCharge()
                        else
                            result = item.BookingFee; //Cancelled before two hours - should be the same as GetTotalCharge()
                    }
                    else
                    {
                        if (item.IsStarted)
                            result = item.UsageFeeCharged + item.OverTimePenaltyFee + (item.ResourceRate == 0 ? 0 : item.ReservationFee2); //should be the same as GetTotalCharge()
                        else
                            result = item.UncancelledPenaltyFee; //should be the same as GetTotalCharge()
                    }
                }
                else
                {
                    if (item.IsStarted)
                        result = item.UsageFeeCharged + item.OverTimePenaltyFee + (item.ResourceRate == 0 ? 0 : item.ReservationFee2); //should be the same as GetTotalCharge()
                    else
                        result = item.UncancelledPenaltyFee + item.ReservationFee2; //should be the same as GetTotalCharge()
                }
            }

            return result;
        }

        /// <summary>
        /// The final billed amount based on BillingType and Room.
        /// </summary>
        public decimal GetLineCost(Models.Billing.IRoomBilling item)
        {
            // [2015-11-13 jg] this is identical to the logic originally in:
            //      1) sselFinOps.AppCode.BLL.FormulaBL.ApplyRoomFormula (for External Invoice)
            //      2) sselIndReports.AppCode.Bll.RoomBillingBL.GetRoomBillingDataByClientID (for User Usage Summary)
            //      3) LNF.WebApi.Billing.Models.ReportUtility.ApplyRoomFormula (for SUB reports)

            decimal result = 0;

            int cleanRoomId = 6;
            int organicsBayId = 6;

            //1. Find out all Monthly type users and apply to Clean room
            if (IsMonthlyUserBillingType(item.BillingTypeID))
            {
                if (item.RoomID == cleanRoomId) //Clean Room
                    result = item.MonthlyRoomCharge;
                else
                    result = item.GetTotalCharge();
            }
            //2. The growers are charged with room fee only when they reserve and activate a tool
            else if (IsGrowerUserBillingType(item.BillingTypeID))
            {
                if (item.RoomID == organicsBayId) //Organics Bay
                    result = item.RoomCharge; //organics bay must be charged for growers as well
                else
                    result = item.AccountDays * item.RoomRate + item.EntryCharge;
            }
            else if (item.BillingTypeID == BillingType.Other)
            {
                result = 0;
            }
            else if (item.BillingTypeID == BillingType.Grower_Observer)
            {
                result = item.GetTotalCharge();
            }
            else
            {
                //Per Use types
                result = item.GetTotalCharge();
            }

            return result;
        }

        /// <summary>
        /// Calculate the true room cost based on billing types.
        /// </summary>
        public void CalculateRoomLineCost(DataTable dt)
        {
            if (!dt.Columns.Contains("LineCost"))
                dt.Columns.Add("LineCost", typeof(decimal));

            if (!dt.Columns.Contains("DailyFee"))
                dt.Columns.Add("DailyFee", typeof(decimal));

            if (!dt.Columns.Contains("EntryFee"))
                dt.Columns.Add("EntryFee", typeof(decimal));

            if (!dt.Columns.Contains("Room"))
                dt.Columns.Add("Room", typeof(string));

            foreach (DataRow dr in dt.Rows)
            {
                var item = RoomBillingUtility.CreateRoomBillingFromDataRow(dr, false).CreateModel<Models.Billing.IRoomBilling>();

                dr.SetField("Room", RoomUtility.GetRoomDisplayName(item.RoomID));

                if (item.RoomCharge > 0)
                    dr.SetField("DailyFee", item.RoomCharge);

                if (item.EntryCharge > 0)
                    dr.SetField("EntryFee", item.EntryCharge);

                dr.SetField("LineCost", GetLineCost(item));
            }
        }

        /// <summary>
        /// Calculate the true tool cost based on billing types.
        /// </summary>
        public void CalculateToolLineCost(DataTable dt)
        {
            if (!dt.Columns.Contains("LineCost"))
                dt.Columns.Add("LineCost", typeof(decimal));

            if (!dt.Columns.Contains("Room"))
                dt.Columns.Add("Room", typeof(string));

            //Part I: Get the true cost based on billing types
            foreach (DataRow dr in dt.Rows)
            {
                var item = ToolBillingUtility.CreateToolBillingFromDataRow(dr, false);
                dr.SetField("Room", RoomUtility.GetRoomDisplayName(item.RoomID));
                dr.SetField("LineCost", GetLineCost(item));
            }
        }

        public IEnumerable<IBillingType> GetAllBillingTypes()
        {
            if (_allBillingTypes == null)
                _allBillingTypes = Session.Query<BillingType>().ToList();
            return _allBillingTypes.CreateModels<IBillingType>();
        }

        public IBillingType Default => Regular;

        public IBillingType Int_Ga
        {
            get { return Find(BillingTypeItem.Int_Ga); }
        }

        public IBillingType Int_Si
        {
            get { return Find(BillingTypeItem.Int_Si); }
        }

        public IBillingType Int_Hour
        {
            get { return Find(BillingTypeItem.Int_Hour); }
        }

        public IBillingType Int_Tools
        {
            get { return Find(BillingTypeItem.Int_Tools); }
        }

        public IBillingType ExtAc_Ga
        {
            get { return Find(BillingTypeItem.ExtAc_Ga); }
        }

        public IBillingType ExtAc_Si
        {
            get { return Find(BillingTypeItem.ExtAc_Si); }
        }

        public IBillingType ExtAc_Tools
        {
            get { return Find(BillingTypeItem.ExtAc_Tools); }
        }

        public IBillingType ExtAc_Hour
        {
            get { return Find(BillingTypeItem.ExtAc_Hour); }
        }

        public IBillingType NonAc
        {
            get { return Find(BillingTypeItem.NonAc); }
        }

        public IBillingType NonAc_Tools
        {
            get { return Find(BillingTypeItem.NonAc_Tools); }
        }

        public IBillingType NonAc_Hour
        {
            get { return Find(BillingTypeItem.NonAc_Hour); }
        }

        public IBillingType Regular
        {
            get { return Find(BillingTypeItem.Regular); }
        }

        public IBillingType Grower_Observer
        {
            get { return Find(BillingTypeItem.Grower_Observer); }
        }

        public IBillingType Remote
        {
            get { return Find(BillingTypeItem.Remote); }
        }

        public IBillingType RegularException
        {
            get { return Find(BillingTypeItem.RegularException); }
        }

        public IBillingType Other
        {
            get { return Find(BillingTypeItem.Other); }
        }
    }
}
