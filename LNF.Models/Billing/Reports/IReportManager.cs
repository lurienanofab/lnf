using LNF.Models.Billing.Reports.ServiceUnitBilling;
using System;
using System.Collections.Generic;

namespace LNF.Models.Billing.Reports
{
    public interface IReportManager
    {
        IEnumerable<IBillingSummary> GetBillingSummary(DateTime sd, DateTime ed, bool includeRemote = false, int clientId = 0);
        IEnumerable<IRegularException> GetRegularExceptions(DateTime period, int clientId = 0);
        RoomJU GetRoomJU(DateTime sd, DateTime ed, string type, int id = 0);
        RoomSUB GetRoomSUB(DateTime sd, DateTime ed, int id = 0);
        StoreSUB GetStoreSUB(DateTime sd, DateTime ed, int id = 0, string option = null);
        ToolJU GetToolJU(DateTime sd, DateTime ed, string type, int id = 0);
        ToolSUB GetToolSUB(DateTime sd, DateTime ed, int id = 0);
        IEnumerable<FinancialManagerReportEmail> GetFinancialManagerReportEmails(FinancialManagerReportOptions options);
        SendMonthlyUserUsageEmailsProcessResult SendFinancialManagerReport(FinancialManagerReportOptions options);
        IEnumerable<UserApportionmentReportEmail> GetUserApportionmentReportEmails(UserApportionmentReportOptions options);

        /// <summary>
        /// Sends clients alerts at the beginning of the month including 1) anti-passback errors in the data, and 2) need to apportion.
        /// </summary>
        SendMonthlyApportionmentEmailsProcessResult SendUserApportionmentReport(UserApportionmentReportOptions options);
    }
}