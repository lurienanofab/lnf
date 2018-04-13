using LNF.CommonTools;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Billing
{
    public class BillingTypeManager : ManagerBase
    {
        public BillingTypeManager(ISession session) : base(session){}

        public string GetBillingTypeName(BillingType billingType)
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

        public BillingType GetBillingType(string text)
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

        private BillingType GetBillingType(Client client, Account account, IEnumerable<ClientOrg> clientOrgs, IEnumerable<ClientAccount> clientAccounts, IEnumerable<ClientRemote> clientRemotes, IEnumerable<ClientOrgBillingTypeLog> cobtLogs)
        {
            //assume that the collections passed have already been filtered for start and end dates

            int record = 0;
            BillingType result = null;

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
                    result = cobtlog.BillingType;
                if (result == null)
                    result = Regular;
            }

            return result;
        }

        public BillingType GetBillingType(Client client, Account account, DateTime period)
        {
            // always add one more month for @Period, because we allow changes made during the current month that will take effect
            // as long as it's before the 4th business day of business
            // 2011-01-26 the above statement is not quite right.  We should not allow change after the Period.  if a change is made on 2011-01-04, it has nothing
            // to do with period = 2010-12-01
            //set @Period = dbo.udf_BusinessDate (DATEADD(MONTH, 1, @Period), null)

            DateTime sd = period;
            DateTime ed = sd.AddMonths(1);

            var mgr = Session.ActiveDataItemManager();

            return GetBillingType(
                client,
                account,
                mgr.FindActive(Session.Query<ClientOrg>().Where(x => x.Client == client && x.Org == account.Org), x => x.ClientOrgID, sd, ed),
                mgr.FindActive(Session.Query<ClientAccount>().Where(x => x.ClientOrg.Client == client && x.Account == account), x => x.ClientAccountID, sd, ed),
                mgr.FindActive(Session.Query<ClientRemote>().Where(x => x.Client == client && x.Account == account), x => x.ClientRemoteID, sd, ed),
                ClientOrgBillingTypeLogUtility.GetActive(sd, ed).Where(x => x.ClientOrg.Client == client && x.ClientOrg.Org == account.Org).ToArray()
            );
        }

        public void Update(Client client, DateTime period, BillingDataProcessStep1 step1)
        {
            bool isTemp = RepositoryUtility.IsCurrentPeriod(period);
            step1.PopulateToolBilling(period, client.ClientID, isTemp);
            step1.PopulateRoomBilling(period, client.ClientID, isTemp);
        }

        public IList<IToolBilling> SelectToolBillingData<T>(Client client, DateTime period) where T : IToolBilling
        {
            string sql = "EXEC Billing.dbo.ToolData_Select @Action='ForToolBilling', @Period=:period, @ClientID=:ClientID";
            var query = Session.SqlQuery(sql).SetParameter("period", period).SetParameter("ClientID", client.ClientID).List<T>();
            var result = query.Select(x => x as IToolBilling).ToList();
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

        public BillingType Find(int billingTypeId)
        {
            return Session.Single<BillingType>(billingTypeId);
        }

        public BillingType Find(string name)
        {
            return Session.Query<BillingType>().FirstOrDefault(x => x.BillingTypeName == name);
        }

        public BillingType GetBillingTypeByClientAndOrg(DateTime period, Client client, Org org)
        {
            // always add one more month for period, because we allow changes made during the current month that will take effect
            // as long as it's before the 4th business day of business
            DateTime p = Utility.NextBusinessDay(period.AddMonths(1));

            var cobtLog = Session.Query<ClientOrgBillingTypeLog>().FirstOrDefault(x => x.ClientOrg.Client == client && x.ClientOrg.Org == org && x.EffDate < p && (x.DisableDate == null || x.DisableDate > p));

            if (cobtLog != null)
                return cobtLog.BillingType;
            else
                return Default;
        }

        /// <summary>
        /// The final billed amount based on BillingType and Tool.
        /// </summary>
        public decimal GetLineCost(IToolBilling item)
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
            else if (item.BillingTypeID == BillingType.Other)
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
        public decimal GetLineCost(IRoomBilling item)
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
                IRoomBilling item = RoomBillingUtility.CreateRoomBillingFromDataRow(dr, false);

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
                IToolBilling item = ToolBillingUtility.CreateToolBillingFromDataRow(dr, false);
                dr.SetField("Room", RoomUtility.GetRoomDisplayName(item.RoomID));
                dr.SetField("LineCost", GetLineCost(item));
            }
        }

        public BillingType Default
        {
            get { return Regular; }
        }

        public BillingType Int_Ga
        {
            get { return Find(BillingType.Int_Ga); }
        }

        public BillingType Int_Si
        {
            get { return Find(BillingType.Int_Si); }
        }

        public BillingType Int_Hour
        {
            get { return Find(BillingType.Int_Hour); }
        }

        public BillingType Int_Tools
        {
            get { return Find(BillingType.Int_Tools); }
        }

        public BillingType ExtAc_Ga
        {
            get { return Find(BillingType.ExtAc_Ga); }
        }

        public BillingType ExtAc_Si
        {
            get { return Find(BillingType.ExtAc_Si); }
        }

        public BillingType ExtAc_Tools
        {
            get { return Find(BillingType.ExtAc_Tools); }
        }

        public BillingType ExtAc_Hour
        {
            get { return Find(BillingType.ExtAc_Hour); }
        }

        public BillingType NonAc
        {
            get { return Find(BillingType.NonAc); }
        }

        public BillingType NonAc_Tools
        {
            get { return Find(BillingType.NonAc_Tools); }
        }

        public BillingType NonAc_Hour
        {
            get { return Find(BillingType.NonAc_Hour); }
        }

        public BillingType Regular
        {
            get { return Find(BillingType.Regular); }
        }

        public BillingType Grower_Observer
        {
            get { return Find(BillingType.Grower_Observer); }
        }

        public BillingType Remote
        {
            get { return Find(BillingType.Remote); }
        }

        public BillingType RegularException
        {
            get { return Find(BillingType.RegularException); }
        }

        public BillingType Other
        {
            get { return Find(BillingType.Other); }
        }
    }
}
