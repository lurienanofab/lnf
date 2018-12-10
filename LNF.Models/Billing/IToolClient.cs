using System;
using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IToolClient
    {
        IEnumerable<ToolBillingItem> CreateToolBilling(DateTime period, int clientId = 0);
        IEnumerable<ToolBillingItem> CreateToolBilling(int reservationId);
        IEnumerable<ToolDataItem> CreateToolData(DateTime period, int clientId = 0, int resourceId = 0);
        IEnumerable<ToolDataItem> CreateToolData(int reservationId);
        IEnumerable<ToolBillingItem> GetToolBilling(DateTime period, int clientId = 0, int resourceId = 0);
        IEnumerable<ToolBillingItem> GetToolBilling(int reservationId);
        IEnumerable<ToolDataItem> GetToolData(DateTime period, int clientId = 0, int resourceId = 0);
        IEnumerable<ToolDataItem> GetToolData(int reservationId);
        IEnumerable<ToolDataCleanItem> GetToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0);
        ToolDataCleanItem GetToolDataClean(int reservationId);
    }
}