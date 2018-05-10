using LNF.Billing;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Reporting
{
    public class ClientDatabaseReport : DefaultReport<UserCriteria>
    {
        protected IClientManager ClientManager => DA.Use<IClientManager>();
        protected IAccountManager AccountManager => DA.Use<IAccountManager>();
        protected IActiveLogManager ActiveLogManager => DA.Use<IActiveLogManager>();

        public override string Key { get { return "client-database-report"; } }
        public override string Title { get { return "Client Database Report"; } }
        public override string CategoryName { get { return "Database Reports"; } }

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

        public IList<Client> GetClientsInPeriod()
        {
            IList<ActiveLog> logs = ActiveLogManager.Range("Client", Criteria.Period, Criteria.Period.AddMonths(1));
            int[] records = logs.Select(x => x.Record).ToArray();
            IList<Client> result = DA.Current.Query<Client>().Where(x => records.Contains(x.ClientID)).ToList();
            return result;
        }

        public void GetClientList(ref GenericResult result)
        {
            ArrayList list = new ArrayList();
            IEnumerable<Client> query = GetClientsInPeriod();
            IEnumerable<ActiveLogItem<ClientOrg>> activeClientOrgs = new DateRange(Criteria.Period).Items<ClientOrg>(x => new ActiveLogKey("ClientOrg", x.ClientOrgID));
            IEnumerable<Priv> privs = DA.Current.Query<Priv>();
            foreach (Client c in query)
            {
                string[] item = new string[]
                {
                    c.ClientID.ToString(),
                    c.UserName,
                    c.DisplayName,
                    "<div class=\"list-subitem\">" + string.Join("</div><div class=\"list-subitem\">", c.Roles()) + "</div><div style=\"clear: both;\"></div>",
                    "<div class=\"list-subitem\">" + string.Join("</div><div class=\"list-subitem\">", activeClientOrgs.Where(x=>x.Item.Client == c).Select(x=>x.Item.Org.OrgName)) + "</div><div style=\"clear: both;\"></div>"
                };
                list.Add(item);
            }
            result.Data = list;
        }

        public void GetDetail(ref GenericResult result)
        {
            Client c = DA.Current.Single<Client>(Criteria.ClientID);
            if (c != null)
            {
                var data = new
                {
                    Error = "",
                    Client = GetDetailClientInfo(c),
                    Orgs = GetDetailOrgInfo(ClientManager.ClientOrgs(c).ToList())
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

        public ArrayList GetDetailOrgInfo(IEnumerable<ClientOrg> corgs)
        {
            ArrayList list = new ArrayList();
            foreach (ClientOrg co in corgs)
            {
                if (ActiveLogManager.IsActive(co, Criteria.Period, Criteria.Period.AddMonths(1)))
                {
                    var item = new
                    {
                        OrgName = co.Org.OrgName,
                        Department = co.Department.DepartmentName,
                        Role = co.Role.RoleName,
                        Phone = co.Phone,
                        Email = co.Email,
                        Managers = GetDetailManagerInfo(ClientManagerUtility.FindManagers(co.ClientOrgID)),
                        BillingType = GetDetailBillingType(co),
                        Accounts = GetDetailAccounts(AccountManager.FindClientAccounts(co.ClientOrgID))
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        public string[] GetDetailManagerInfo(IEnumerable<Repository.Data.ClientManager> mgrs)
        {
            List<string> list = new List<string>();
            foreach (var cm in mgrs)
            {
                if (ActiveLogManager.IsActive(cm, Criteria.Period, Criteria.Period.AddMonths(1)))
                {
                    list.Add(cm.ManagerOrg.Client.DisplayName);
                }
            }
            return list.ToArray();
        }

        public string GetDetailBillingType(ClientOrg co)
        {
            ClientOrgBillingTypeLog ts = ClientOrgBillingTypeLogUtility.GetActive(Criteria.Period, Criteria.Period.AddMonths(1)).FirstOrDefault(x => x.ClientOrg == co);
            return ts.BillingType.BillingTypeName;
        }

        public string[] GetDetailAccounts(IEnumerable<ClientAccount> caccts)
        {
            List<string> list = new List<string>();
            foreach (ClientAccount ca in caccts)
            {
                if (ActiveLogManager.IsActive(ca, Criteria.Period, Criteria.Period.AddMonths(1)))
                    list.Add(ca.Account.Name);
            }
            return list.ToArray();
        }

        public object GetDetailClientInfo(Client c)
        {
            var result = new
            {
                FName = c.FName,
                MName = c.MName,
                LName = c.LName,
                UserName = c.UserName,
                ClientID = c.ClientID,
                PrivList = c.Roles(),
                Citizen = DA.Current.Single<DemCitizen>(c.DemCitizenID).DemCitizenValue,
                Gender = DA.Current.Single<DemGender>(c.DemGenderID).DemGenderValue,
                Race = DA.Current.Single<DemRace>(c.DemRaceID).DemRaceValue,
                Ethnicity = DA.Current.Single<DemEthnic>(c.DemEthnicID).DemEthnicValue,
                Disability = DA.Current.Single<DemDisability>(c.DemDisabilityID).DemDisabilityValue,
                TechnicalField = DA.Current.Single<TechnicalField>(c.TechnicalFieldID).TechnicalFieldName,
                CommunityList = CommunityUtility.GetCommunityNames(c.Communities)
            };
            return result;
        }
    }
}
