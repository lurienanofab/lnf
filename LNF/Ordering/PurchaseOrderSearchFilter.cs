using LNF.CommonTools;
using LNF.Repository;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace LNF.Ordering
{
    public class PurchaseOrderSearchFilter
    {
        //private bool _refresh;

        public int ClientID { get; }
        public bool IncludeSelf { get; }
        public int OtherClientID { get; }
        public int VendorID { get; }
        public string VendorNameList { get; }
        public string VendorNameText { get; }
        public string VendorSearchType { get; }
        public string Keywords { get; }
        public string PartNum { get; }
        public int[] StatusList { get; }
        public int POID { get; }
        public int DisplayOption { get; }
        public string ShortCode { get; }
        public int Skip { get; }
        public int Take { get; }

        [Obsolete]
        public PurchaseOrderSearchFilter()
        {
            ClientID = Providers.Context.Current.GetRequestValueOrDefault("clientId", -999);
            IncludeSelf = bool.Parse(Providers.Context.Current.GetRequestValueOrDefault("includeSelf", "true"));
            OtherClientID = Providers.Context.Current.GetRequestValueOrDefault("otherClientId", -999);
            VendorID = Providers.Context.Current.GetRequestValueOrDefault("vendorId", -1);
            VendorNameList = Providers.Context.Current.GetRequestValueOrDefault("vendorNameList", string.Empty);
            VendorNameText = Providers.Context.Current.GetRequestValueOrDefault("vendorNameText", string.Empty);
            VendorSearchType = Providers.Context.Current.GetRequestValueOrDefault("vendorSearchType", string.Empty);
            Keywords = Providers.Context.Current.GetRequestValueOrDefault("keywords", string.Empty);
            PartNum = Providers.Context.Current.GetRequestValueOrDefault("partNum", string.Empty);
            StatusList = GetStatusListFromString(Providers.Context.Current.GetRequestValueOrDefault("statusIdList", string.Empty));
            POID = Providers.Context.Current.GetRequestValueOrDefault("poid", 0);
            DisplayOption = Providers.Context.Current.GetRequestValueOrDefault("displayOption", 0);
            ShortCode = Providers.Context.Current.GetRequestValueOrDefault("shortCode", string.Empty);
            //_refresh = bool.Parse(Providers.Context.Current.GetRequestValueOrDefault("refresh", "false"));
        }

        private int[] GetStatusListFromString(string list)
        {
            if (string.IsNullOrEmpty(list)) return null;
            int[] result = list.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            return result;
        }

        public PurchaseOrderSearchFilter(NameValueCollection nvc)
        {
            ClientID = nvc.Value("clientId", -999);
            IncludeSelf = nvc.BoolValue("includeSelf", true);
            OtherClientID = nvc.Value("otherClientId", -999);
            VendorID = nvc.Value("vendorId", -1);
            VendorNameList = nvc.Value("vendorNameList", string.Empty);
            VendorNameText = nvc.Value("vendorNameText", string.Empty);
            VendorSearchType = nvc.Value("vendorSearchType", string.Empty);
            Keywords = nvc.Value("keywords", string.Empty);
            PartNum = nvc.Value("partNum", string.Empty);
            StatusList = GetStatusListFromString(nvc.Value("statusIdList", string.Empty));
            POID = nvc.Value("poid", 0);
            DisplayOption = nvc.Value("displayOption", 0);
            ShortCode = nvc.Value("shortCode", string.Empty);
            //_refresh = nvc.Value("refresh", false);
        }

        //public bool Refresh()
        //{
        //    return _refresh;
        //}
    }
}
