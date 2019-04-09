using LNF.Models.Billing.Reports.ServiceUnitBilling;
using System;
using System.Collections.Generic;

namespace LNF.Models.Billing.Reports
{
    public interface IReportManager
    {
        IEnumerable<BillingSummaryItem> GetBillingSummary(DateTime sd, DateTime ed, bool includeRemote = false, int clientId = 0);
        IEnumerable<RegularExceptionItem> GetRegularExceptions(DateTime period, int clientId = 0);
        RoomJU GetRoomJU(DateTime sd, DateTime ed, string type, int id = 0);
        RoomSUB GetRoomSUB(DateTime sd, DateTime ed, int id = 0);
        StoreSUB GetStoreSUB(DateTime sd, DateTime ed, int id = 0, string option = null);
        ToolJU GetToolJU(DateTime sd, DateTime ed, string type, int id = 0);
        ToolSUB GetToolSUB(DateTime sd, DateTime ed, int id = 0);
        SendMonthlyUserUsageEmailsProcessResult SendFinancialManagerReport(FinancialManagerReportOptions options);
        IEnumerable<FinancialManagerReportEmail> ViewFinancialManagerReport(DateTime period, int clientId = 0, int managerOrgId = 0, string message = null);
        SendMonthlyApportionmentEmailsProcessResult SendUserApportionmentReport(UserApportionmentReportOptions options);
        IEnumerable<UserApportionmentReportEmail> ViewUserApportionmentReport(DateTime period, string message = null);
    }
}