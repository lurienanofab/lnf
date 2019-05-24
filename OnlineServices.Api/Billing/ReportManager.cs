using LNF.Models.Billing.Reports;
using LNF.Models.Billing.Reports.ServiceUnitBilling;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class ReportManager : ApiClient, IReportManager
    {
        public IEnumerable<IBillingSummary> GetBillingSummary(DateTime sd, DateTime ed, bool includeRemote = false, int clientId = 0)
        {
            return Get<List<BillingSummaryItem>>("webapi/billing/report/billing-summary", QueryStrings(new { sd, ed, includeRemote, clientId }));
        }

        public IEnumerable<IRegularException> GetRegularExceptions(DateTime period, int clientId = 0)
        {
            return Get<List<RegularExceptionItem>>("webapi/billing/report/regular-exception", QueryStrings(new { period, clientId }));
        }

        public RoomJU GetRoomJU(DateTime sd, DateTime ed, string type, int id = 0)
        {
            return Get<RoomJU>("webapi/billing/report/room/ju/{type}", UrlSegments(new { type }) & QueryStrings(new { sd, ed, id }));
        }

        public RoomSUB GetRoomSUB(DateTime sd, DateTime ed, int id = 0)
        {
            return Get<RoomSUB>("webapi/billing/report/room/sub", QueryStrings(new { sd, ed, id }));
        }

        public StoreSUB GetStoreSUB(DateTime sd, DateTime ed, int id = 0, string option = null)
        {
            var parameters = QueryStrings(new { sd, ed, id });

            if (!string.IsNullOrEmpty(option))
                parameters.Add("option", option, ParameterType.QueryString);

            return Get<StoreSUB>("webapi/billing/report/store/sub", parameters);
        }

        public ToolJU GetToolJU(DateTime sd, DateTime ed, string type, int id = 0)
        {
            return Get<ToolJU>("webapi/billing/report/tool/ju/{type}", UrlSegments(new { type }) & QueryStrings(new { sd, ed, id }));
        }

        public ToolSUB GetToolSUB(DateTime sd, DateTime ed, int id = 0)
        {
            return Get<ToolSUB>("webapi/billing/report/tool/sub", QueryStrings(new { sd, ed, id }));
        }

        public SendMonthlyUserUsageEmailsProcessResult SendFinancialManagerReport(FinancialManagerReportOptions options)
        {
            return Post<SendMonthlyUserUsageEmailsProcessResult>("webapi/billing/report/financial-manager", options);
        }

        public IEnumerable<FinancialManagerReportEmail> GetFinancialManagerReportEmails(FinancialManagerReportOptions options)
        {
            return Post<List<FinancialManagerReportEmail>>("webapi/billing/report/financial-manager/view", options);
        }

        public SendMonthlyApportionmentEmailsProcessResult SendUserApportionmentReport(UserApportionmentReportOptions options)
        {
            return Post<SendMonthlyApportionmentEmailsProcessResult>("webapi/billing/report/user-apportionment", options);
        }

        public IEnumerable<UserApportionmentReportEmail> GetUserApportionmentReportEmails(UserApportionmentReportOptions options)
        {
            return Post<List<UserApportionmentReportEmail>>("webapi/billing/report/user-apportionment/view", options);
        }
    }
}
