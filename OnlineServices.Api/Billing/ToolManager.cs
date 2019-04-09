using LNF.Models.Billing;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class ToolManager : ApiClient, IToolManager
    {
        public IEnumerable<ToolBillingItem> CreateToolBilling(DateTime period, int clientId = 0)
        {
            return Get<List<ToolBillingItem>>("webapi/billing/tool/create", QueryStrings(new { period, clientId }));
        }

        public IEnumerable<ToolBillingItem> CreateToolBilling(int reservationId)
        {
            return Get<List<ToolBillingItem>>("webapi/billing/tool/create/{reservationId}", UrlSegments(new { reservationId }));
        }

        public IEnumerable<ToolDataItem> CreateToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            return Get<List<ToolDataItem>>("webapi/billing/tool/data/create", QueryStrings(new { period, clientId, resourceId }));
        }

        public IEnumerable<ToolDataItem> CreateToolData(int reservationId)
        {
            return Get<List<ToolDataItem>>("webapi/billing/tool/data/create/{reservationId}", UrlSegments(new { reservationId }));
        }

        public IEnumerable<ToolBillingItem> GetToolBilling(DateTime period, int clientId = 0, int resourceId = 0)
        {
            return Get<List<ToolBillingItem>>("webapi/billing/tool", QueryStrings(new { period, clientId, resourceId }));
        }

        public IEnumerable<ToolBillingItem> GetToolBilling(int reservationId)
        {
            return Get<List<ToolBillingItem>>("webapi/billing/tool/{reservationId}", UrlSegments(new { reservationId }));
        }

        public IEnumerable<ToolDataItem> GetToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            return Get<List<ToolDataItem>>("webapi/billing/tool/data", QueryStrings(new { period, clientId, resourceId }));
        }

        public IEnumerable<ToolDataItem> GetToolData(int reservationId)
        {
            return Get<List<ToolDataItem>>("webapi/billing/tool/data/{reservationId}", UrlSegments(new { reservationId }));
        }

        public IEnumerable<ToolDataCleanItem> GetToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0)
        {
            return Get<List<ToolDataCleanItem>>("webapi/billing/tool/data/clean", QueryStrings(new { sd, ed, clientId, resourceId }));
        }

        public ToolDataCleanItem GetToolDataClean(int reservationId)
        {
            return Get<ToolDataCleanItem>("webapi/billing/tool/data/clean/{reservationId}", UrlSegments(new { reservationId }));
        }
    }
}
