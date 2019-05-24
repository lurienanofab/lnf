using LNF.Models;
using LNF.Models.Data;
using LNF.Models.Data.Utility;
using LNF.Models.Data.Utility.BillingChecks;
using LNF.Repository;
using LNF.Repository.Data;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class UtilityManager : ManagerBase, IUtilityManager
    {
        public UtilityManager(IProvider provider) : base(provider) { }

        public int FixAllAutoEndProblems(DateTime period)
        {
            var count = DA.Command()
                .Param("Period", period)
                .ExecuteScalar<int>("dbo.BillingChecks_FixAutoEndProblems").Value;

            return count;
        }

        public int FixAutoEndProblem(DateTime period, int reservationId)
        {
            var count = DA.Command()
                .Param("Period", period)
                .Param("ReservationID", reservationId)
                .ExecuteScalar<int>("dbo.BillingChecks_FixAutoEndProblems").Value;

            return count;
        }

        public IEnumerable<AutoEndProblem> GetAutoEndProblems(DateTime period)
        {
            var dt = DA.Command()
                .Param("Period", period)
                .FillDataTable("dbo.BillingChecks_FindAutoEndProblems");

            var result = new List<AutoEndProblem>();

            foreach (DataRow dr in dt.Rows)
            {
                var item = new AutoEndProblem
                {
                    ReservationID = dr.Field<int>("ReservationID"),
                    AutoEndType = dr.Field<string>("AutoEndType"),
                    AutoEndReservation = dr.Field<bool>("AutoEndReservation"),
                    AutoEndResource = TimeSpan.FromMinutes(dr.Field<int>("AutoEndResource")),
                    Duration = TimeSpan.FromMinutes(dr.Field<double>("Duration")),
                    EndDateTime = dr.Field<DateTime>("EndDateTime"),
                    ActualEndDateTime = dr.Field<DateTime?>("ActualEndDateTime"),
                    ActualEndDateTimeExpected = dr.Field<DateTime>("ActualEndDateTimeExpected"),
                    Diff = TimeSpan.FromSeconds(dr.Field<int>("DiffSeconds")),
                    ActualEndDateTimeCorrected = dr.Field<DateTime>("ActualEndDateTimeCorrected"),
                    ChargeMultiplier = dr.Field<double>("ChargeMultiplier")
                };

                result.Add(item);
            }

            return result;
        }

        public string GetSiteMenu(int clientId, string target = null) => GetContent("clientId", clientId, target);

        public string GetSiteMenu(string username, string target = null) => GetContent("username", username, target);

        private string GetContent(string paramName, object paramValue, string target)
        {
            return GetRestClient()
                .Execute(GetRestRequest(target)
                .AddQueryParameter(paramName, Convert.ToString(paramValue)))
                .Content;
        }

        private IRestClient GetRestClient()
        {
            return new RestClient(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        }

        private IRestRequest GetRestRequest(string target)
        {
            var req = new RestRequest("webapi/data/ajax/menu", Method.GET, DataFormat.Json);

            if (!string.IsNullOrEmpty(target))
                req.AddQueryParameter("target", target);

            return req;
        }
    }
}
