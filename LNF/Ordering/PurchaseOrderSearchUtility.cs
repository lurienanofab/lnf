using LNF.Cache;
using LNF.DataTables;
using LNF.Models.Ordering;
using LNF.Repository;
using LNF.Repository.Ordering;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace LNF.Ordering
{
    public class PurchaseOrderSearchUtility
    {
        public static DataTablesResult<PurchaseOrderSearchModel> GetDataTablesResult(PurchaseOrderSearchFilter filter)
        {
            IList<PurchaseOrderSearchModel> query = SearchTools.GetQuery(filter);

            if (query == null) return null;

            var result = new DataTablesResult<PurchaseOrderSearchModel>(Providers.Context.Current.PostData);

            result.AddQuery(query)
                .AddProperty(x => x.POID, "LNF{0}")
                .AddProperty(x => x.DisplayName)
                .AddProperty(x => x.ApproverDisplayName)
                .AddProperty(x => x.PartNum)
                .AddProperty(x => x.Description)
                .AddProperty(x => x.VendorName)
                .AddProperty(x => x.CreatedDate, "{0:MM/dd/yyyy}", true)
                .AddProperty(x => x.ShortCode)
                .AddProperty(x => x.TotalPrice, "{0:C}")
                .AddProperty(x => x.StatusName)
                .AddDefaultSort(x => x.CreatedDate, Direction.Descending)
                .Search()
                .Sort()
                .Page()
                .Fill();
            return result;
        }

        public static IList<PurchaseOrderSearch> SearchPO(int clientId, bool includeSelf, int otherClientId, int vendorId, string vendorNameList, string vendorNameText, string vendorSearchType, string keywords, string partNum, int[] statusIdList, int poid, int displayOption, string shortCode)
        {
            //DisplayOption = 0 -> detail
            //DisplayOption = 1 -> summary

            /*
             * We are now passing all parameters explicitly through Providers.DataAccess.ExecuteNamedQuery
             * So we can't rely on default values in the stored proc definition.
             */

            //IncludeSelf comes from a checkbox
            //ClientID comes from the session
            clientId = (includeSelf) ? clientId : -999;

            //OtherClientID comes from a dropdownlist where "View All" = -1
            otherClientId = (otherClientId <= 0) ? -999 : otherClientId;

            string vendorName = null;

            if (vendorSearchType == "text")
            {
                if (!string.IsNullOrEmpty(vendorNameText))
                    vendorName = PurchaseOrderUtility.CleanString(vendorNameText) + "%";
            }
            else if (vendorSearchType == "list")
            {
                if (vendorId > 0)
                    vendorName = PurchaseOrderUtility.CleanString(vendorNameList);
            }

            if (shortCode == null)
                shortCode = string.Empty;

            var query = DA.Current.Query<PurchaseOrderSearch>();

            if (statusIdList != null && statusIdList.Length > 0)
                query = query.Where(x => statusIdList.Contains(x.StatusID));

            if (otherClientId > 0)
                query = query.Where(x => new[] { clientId, otherClientId }.Contains(x.ClientID));

            if (!string.IsNullOrEmpty(vendorName))
                query = query.Where(x => x.CleanVendorName.StartsWith(vendorName));

            if (!string.IsNullOrEmpty(keywords))
                query = query.Where(x => x.Description.Contains(keywords));

            if (!string.IsNullOrEmpty(partNum))
                query = query.Where(x => x.PartNum.Contains(partNum));

            if (poid > 0)
                query = query.Where(x => x.POID == poid);

            if (!string.IsNullOrEmpty(shortCode))
                query = query.Where(x => x.ShortCode.StartsWith(shortCode));

            var result = query.OrderByDescending(x => x.CreatedDate).ThenBy(x => x.POID).ToList();

            return result;
        }

        public static class SearchTools
        {
            //private static string sessionKey = "LNF.Ordering.PurchaseOrderSearchUtility.Query";

            public static NameValueCollection DefaultFilter()
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("clientId", CacheManager.Current.CurrentUser.ClientID.ToString());
                nvc.Add("includeSelf", "true");
                nvc.Add("otherClientId", "-1");
                nvc.Add("vendorId", "-1");
                nvc.Add("vendorNameList", string.Empty);
                nvc.Add("vendorNameText", string.Empty);
                nvc.Add("vendorSearchType", "list");
                nvc.Add("keywords", string.Empty);
                nvc.Add("partNum", string.Empty);
                nvc.Add("statusIdList", string.Empty);
                nvc.Add("poid", "0");
                nvc.Add("displayOption", "1");
                nvc.Add("shortCode", string.Empty);
                return nvc;
            }

            [Obsolete]
            public static IList<PurchaseOrderSearchModel> GetQuery(NameValueCollection nvc)
            {
                PurchaseOrderSearchFilter filter = new PurchaseOrderSearchFilter(nvc);
                return GetQuery(filter);
            }

            [Obsolete]
            public static IList<PurchaseOrderSearchModel> GetQuery()
            {
                PurchaseOrderSearchFilter filter = new PurchaseOrderSearchFilter();
                return GetQuery(filter);
            }

            public static IList<PurchaseOrderSearchModel> GetQuery(PurchaseOrderSearchFilter filter)
            {
                //if (filter.Refresh())
                //    ClearSearchQuery();

                IList<PurchaseOrderSearchModel> query; //= RetrieveSearchQuery();

                //if (query == null)
                //{
                query = SearchPO(filter.ClientID, filter.IncludeSelf, filter.OtherClientID, filter.VendorID, filter.VendorNameList, filter.VendorNameText, filter.VendorSearchType, filter.Keywords, filter.PartNum, filter.StatusList, filter.POID, filter.DisplayOption, filter.ShortCode).Model<PurchaseOrderSearchModel>();
                //StoreSearchQuery(query);
                //}

                return query;
            }

            public static string GetString(object o, bool lower = true)
            {
                if (o == null) return string.Empty;
                if (lower)
                    return o.ToString().ToLower();
                else
                    return o.ToString();
            }

            //public static IList<PurchaseOrderSearchModel> RetrieveSearchQuery()
            //{
            //    IList<PurchaseOrderSearchModel> result = null;
            //    int clientId = CacheManager.Current.ClientID;
            //    var searchResult = PurchaseOrderSearchResult.Get(x => x.ClientID == clientId);
            //    if (searchResult != null)
            //        result = searchResult.Items;
            //    return result;
            //}

            //public static void StoreSearchQuery(IList<PurchaseOrderSearchModel> query)
            //{
            //    PurchaseOrderSearchResult.Set(CacheManager.Current.ClientID, query);
            //}

            //public static void ClearSearchQuery()
            //{
            //    PurchaseOrderSearchResult.Delete(CacheManager.Current.ClientID);
            //}
        }
    }
}
