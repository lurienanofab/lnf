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
    public static class BillingTypeUtility
    {
        public static string GetBillingTypeName(BillingType billingType)
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

        public static BillingType GetBillingType(string text)
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

        private static BillingType GetBillingType(Client client, Account account, IEnumerable<ClientOrg> clientOrgs, IEnumerable<ClientAccount> clientAccounts, IEnumerable<ClientRemote> clientRemotes, IEnumerable<ClientOrgBillingTypeLog> cobtLogs)
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

        public static BillingType GetBillingType(Client client, Account account, DateTime period)
        {
            // always add one more month for @Period, because we allow changes made during the current month that will take effect
            // as long as it's before the 4th business day of business
            // 2011-01-26 the above statement is not quite right.  We should not allow change after the Period.  if a change is made on 2011-01-04, it has nothing
            // to do with period = 2010-12-01
            //set @Period = dbo.udf_BusinessDate (DATEADD(MONTH, 1, @Period), null)

            DateTime sd = period;
            DateTime ed = sd.AddMonths(1);

            return GetBillingType(
                client,
                account,
                DA.Current.Query<ClientOrg>().Where(x => x.Client == client && x.Org == account.Org).FindActive(x => x.ClientOrgID, sd, ed),
                DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.Client == client && x.Account == account).FindActive(x => x.ClientAccountID, sd, ed),
                DA.Current.Query<ClientRemote>().Where(x => x.Client == client && x.Account == account).FindActive(x => x.ClientRemoteID, sd, ed),
                ClientOrgBillingTypeLogUtility.GetActive(sd, ed).Where(x => x.ClientOrg.Client == client && x.ClientOrg.Org == account.Org).ToArray()
            );
        }

        public static void Update(Client client, DateTime period)
        {
            bool isTemp = RepositoryUtility.IsCurrentPeriod(period);
            BillingDataProcessStep1.PopulateToolBilling(period, client.ClientID, isTemp);
            BillingDataProcessStep1.PopulateRoomBilling(period, client.ClientID, isTemp);
        }

        public static IList<IToolBilling> SelectToolBillingData<T>(Client client, DateTime period) where T : IToolBilling
        {
            string sql = "EXEC Billing.dbo.ToolData_Select @Action='ForToolBilling', @Period=:period, @ClientID=:ClientID";
            var query = DA.Current.SqlQuery(sql, new { period, client.ClientID }).List<T>();
            var result = query.Select(x => x as IToolBilling).ToList();
            return result;
        }

        public static bool IsMonthlyUserBillingType(int billingTypeId)
        {
            // BillingType.Int_Ga || BillingType.Int_Si || BillingType.ExtAc_Ga || BillingType.ExtAc_Si
            bool result = new[] { Int_Ga.BillingTypeID, Int_Si.BillingTypeID, ExtAc_Ga.BillingTypeID, ExtAc_Si.BillingTypeID }.Contains(billingTypeId);
            return result;
        }

        public static bool IsGrowerUserBillingType(int billingTypeId)
        {
            // BillingType.Int_Tools || BillingType.ExtAc_Tools || BillingType.NonAc_Tools
            bool result = new[] { Int_Tools.BillingTypeID, ExtAc_Tools.BillingTypeID, NonAc_Tools.BillingTypeID }.Contains(billingTypeId);
            return result;
        }

        public static BillingType Find(int billingTypeId)
        {
            return DA.Current.Single<BillingType>(billingTypeId);
        }

        public static BillingType Find(string name)
        {
            return DA.Current.Query<BillingType>().FirstOrDefault(x => x.BillingTypeName == name);
        }

        public static BillingType GetBillingTypeByClientAndOrg(DateTime period, Client client, Org org)
        {
            // always add one more month for period, because we allow changes made during the current month that will take effect
            // as long as it's before the 4th business day of business
            DateTime p = Utility.NextBusinessDay(period.AddMonths(1));

            var cobtLog = DA.Current.Query<ClientOrgBillingTypeLog>().FirstOrDefault(x => x.ClientOrg.Client == client && x.ClientOrg.Org == org && x.EffDate < p && (x.DisableDate == null || x.DisableDate > p));

            if (cobtLog != null)
                return cobtLog.BillingType;
            else
                return Default;
        }

        public static BillingType Default
        {
            get { return Regular; }
        }

        public static BillingType Int_Ga
        {
            get { return Find(BillingType.Int_Ga); }
        }

        public static BillingType Int_Si
        {
            get { return Find(BillingType.Int_Si); }
        }

        public static BillingType Int_Hour
        {
            get { return Find(BillingType.Int_Hour); }
        }

        public static BillingType Int_Tools
        {
            get { return Find(BillingType.Int_Tools); }
        }

        public static BillingType ExtAc_Ga
        {
            get { return Find(BillingType.ExtAc_Ga); }
        }

        public static BillingType ExtAc_Si
        {
            get { return Find(BillingType.ExtAc_Si); }
        }

        public static BillingType ExtAc_Tools
        {
            get { return Find(BillingType.ExtAc_Tools); }
        }

        public static BillingType ExtAc_Hour
        {
            get { return Find(BillingType.ExtAc_Hour); }
        }

        public static BillingType NonAc
        {
            get { return Find(BillingType.NonAc); }
        }

        public static BillingType NonAc_Tools
        {
            get { return Find(BillingType.NonAc_Tools); }
        }

        public static BillingType NonAc_Hour
        {
            get { return Find(BillingType.NonAc_Hour); }
        }

        public static BillingType Regular
        {
            get { return Find(BillingType.Regular); }
        }

        public static BillingType Grower_Observer
        {
            get { return Find(BillingType.Grower_Observer); }
        }

        public static BillingType Remote
        {
            get { return Find(BillingType.Remote); }
        }

        public static BillingType RegularException
        {
            get { return Find(BillingType.RegularException); }
        }

        public static BillingType Other
        {
            get { return Find(BillingType.Other); }
        }
    }
}
