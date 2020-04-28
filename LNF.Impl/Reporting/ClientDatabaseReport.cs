using LNF.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Reporting
{
    public class ClientDatabaseReport : DefaultReport<UserCriteria>
    {
        public IProvider Provider { get; }
        public override string Key { get { return "client-database-report"; } }
        public override string Title { get { return "Client Database Report"; } }
        public override string CategoryName { get { return "Database Reports"; } }

        public ClientDatabaseReport(IProvider provider)
        {
            Provider = provider;
        }

        public override void WriteCriteria(StringBuilder sb)
        {
            Criteria.CreateWriter(sb)
                .WriteText("Period:\n")
                .WriteMonthSelect()
                .WriteYearSelect()
                .WriteButton("refresh", "Refresh", new { @class = "refresh-button" });
        }

        public override GenericResult Execute(ResultType resultType)
        {
            GenericResult result = new GenericResult
            {
                Success = true,
                Message = string.Empty
            };

            GetReportData(ref result);

            return result;
        }

        public void GetReportData(ref GenericResult result)
        {
            if (Criteria.ClientID == 0)
                GetClientList(ref result);
            else
                GetDetail(ref result);
        }

        public IEnumerable<IClient> GetClientsInPeriod()
        {
            var result = Provider.Data.Client.GetActiveClients(Criteria.Period, Criteria.Period.AddMonths(1));
            return result;
        }

        public void GetClientList(ref GenericResult result)
        {
            ArrayList list = new ArrayList();
            IEnumerable<IClient> query = GetClientsInPeriod();
            IEnumerable<IClient> activeClientOrgs = Provider.Data.Client.GetActiveClientOrgs(Criteria.Period, Criteria.Period.AddMonths(1));
            IEnumerable<IPriv> privs = Provider.Data.Client.GetPrivs();
            foreach (var c in query)
            {
                string[] item = new string[]
                {
                    c.ClientID.ToString(),
                    c.UserName,
                    c.DisplayName,
                    "<div class=\"list-subitem\">" + string.Join("</div><div class=\"list-subitem\">", c.Roles()) + "</div><div style=\"clear: both;\"></div>",
                    "<div class=\"list-subitem\">" + string.Join("</div><div class=\"list-subitem\">", activeClientOrgs.Where(x => x.ClientID == c.ClientID).Select(x => x.OrgName)) + "</div><div style=\"clear: both;\"></div>"
                };
                list.Add(item);
            }
            result.Data = list;
        }

        public void GetDetail(ref GenericResult result)
        {
            IClient c = Provider.Data.Client.GetClient(Criteria.ClientID);
            if (c != null)
            {
                var data = new
                {
                    Error = "",
                    Client = GetDetailClientInfo(c),
                    Orgs = GetDetailOrgInfo(Provider.Data.Client.GetClientOrgs(c.ClientID).ToList())
                };
                result.Data = data;
            }
            else
            {
                var err = new
                {
                    Error = string.Format("ClientID {0} not found.", Criteria.ClientID)
                };
                result.Data = err;
            }
        }

        public ArrayList GetDetailOrgInfo(IEnumerable<IClient> corgs)
        {
            ArrayList list = new ArrayList();
            foreach (var co in corgs)
            {
                if (Provider.Data.ActiveLog.IsActive("ClientOrg", co.ClientOrgID, Criteria.Period, Criteria.Period.AddMonths(1)))
                {
                    var item = new
                    {
                        co.OrgName,
                        Department = co.DepartmentName,
                        Role = co.RoleName,
                        co.Phone,
                        co.Email,
                        Managers = GetDetailManagerInfo(Provider.Data.Client.GetClientManagersByManager(co.ClientOrgID)),
                        BillingType = GetDetailBillingTypeName(co),
                        Accounts = GetDetailAccounts(Provider.Data.Client.GetClientAccounts(co))
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        public string[] GetDetailManagerInfo(IEnumerable<IClientManager> mgrs)
        {
            List<string> list = new List<string>();
            foreach (var cm in mgrs)
            {
                if (Provider.Data.ActiveLog.IsActive("ClientManager", cm.ClientManagerID, Criteria.Period, Criteria.Period.AddMonths(1)))
                {
                    list.Add(cm.ManagerDisplayName);
                }
            }
            return list.ToArray();
        }

        public string GetDetailBillingTypeName(IClient co)
        {
            var ts = ServiceProvider.Current.Billing.BillingType.GetActiveClientOrgBillingTypeLog(co.ClientOrgID, Criteria.Period, Criteria.Period.AddMonths(1));
            var bt = ServiceProvider.Current.Billing.BillingType.GetBillingType(ts.BillingTypeID);
            return bt.BillingTypeName;
        }

        public string[] GetDetailAccounts(IEnumerable<IClientAccount> caccts)
        {
            List<string> list = new List<string>();
            foreach (var ca in caccts)
            {
                if (Provider.Data.ActiveLog.IsActive("ClientAccount", ca.ClientAccountID, Criteria.Period, Criteria.Period.AddMonths(1)))
                    list.Add(ca.AccountName);
            }
            return list.ToArray();
        }

        public object GetDetailClientInfo(IClient c)
        {
            var dem = Provider.Data.Client.GetClientDemographics(c.ClientID);

            var result = new
            {
                c.FName,
                c.MName,
                c.LName,
                c.UserName,
                c.ClientID,
                PrivList = c.Roles(),
                Citizen = dem.DemCitizenValue,
                Gender = dem.DemGenderValue,
                Race = dem.DemRaceValue,
                Ethnicity = dem.DemEthnicValue,
                Disability = dem.DemDisabilityValue,
                TechnicalField = c.TechnicalInterestName,
                CommunityList = CommunityUtility.GetCommunityNames(c.Communities)
            };
            return result;
        }
    }
}
