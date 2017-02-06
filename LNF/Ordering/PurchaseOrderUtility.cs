using LNF.Cache;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Ordering
{
    public enum OrderStatus
    {
        NotSpecified = 0,
        Unordered = 1,
        Ordered = 2,
    }

    public enum ClaimStatus
    {
        All = 0,
        Unclaimed = 1,
        Claimed = 2,
        ClaimedBy = 3
    };

    public static class PurchaseOrderUtility
    {
        public static string DisplayPOID(int poid)
        {
            //TODO: get the prefix from the database
            return "LNF" + poid.ToString();
        }

        public static IList<IOFItem> Items(PurchaseOrder po)
        {
            IList<PurchaseOrderDetail> query = DA.Current.Query<PurchaseOrderDetail>().Where(x => x.PurchaseOrder == po).ToList();
            IList<IOFItem> result = query.Select(x => new IOFItem()
            {
                PODID = x.PODID,
                POID = x.PurchaseOrder.POID,
                ItemID = x.Item.ItemID,
                Quantity = x.Quantity,
                Unit = x.Unit,
                UnitPrice = x.UnitPrice,
                Description = x.Item.Description,
                PartNum = x.Item.PartNum,
                CatID = x.Category.CatID,
                ParentID = x.Category.ParentID,
                CatNo = x.Category.CatNo,
                CreatedDate = x.PurchaseOrder.CreatedDate,
                IsNotes = false
            }).ToList();
            return result;
        }

        public static void SetApprovalPending(PurchaseOrder po)
        {
            po.Status = Status.AwaitingApproval;
        }

        public static bool IsApproved(PurchaseOrder po)
        {
            return !(po.ApprovalDate == null);
        }

        public static bool Approve(PurchaseOrder po, int realApproverClientId, ref string errmsg)
        {
            try
            {
                po.Status = Status.Approved;
                po.RealApproverID = realApproverClientId;
                po.ApprovalDate = DateTime.Now;
                TrackingUtility.Track(TrackingCheckpoints.Approved, po, realApproverClientId);
                return true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        public static bool Reject(PurchaseOrder po, int realApproverClientId, ref string errmsg)
        {
            try
            {
                po.Status = Status.Draft;
                TrackingUtility.Track(TrackingCheckpoints.Rejected, po, realApproverClientId);
                return true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        public static PurchaseOrder Copy(PurchaseOrder po, Account acct = null)
        {
            //INSERT INTO dbo.PurchaseOrder (ClientID, AccountID, VendorID, CreatedDate, NeededDate, ApproverID, Oversized, ShippingMethodID, Notes, Attention, StatusID)
            //SELECT ClientID, AccountID, VendorID, GETDATE(), DATEADD(DAY, 7, GETDATE()), ApproverID, Oversized, ShippingMethodID, Notes, Attention, 1

            PurchaseOrder copy = new PurchaseOrder()
            {
                Client = po.Client,
                AccountID = (acct == null) ? po.AccountID : acct.AccountID,
                Vendor = po.Vendor,
                CreatedDate = DateTime.Now,
                NeededDate = DateTime.Now.AddDays(7),
                Approver = po.Approver,
                Oversized = po.Oversized,
                ShippingMethod = po.ShippingMethod,
                Notes = po.Notes,
                Attention = po.Attention,
                Status = Status.Draft
            };

            DA.Current.Insert(copy);

            IList<PurchaseOrderDetail> details = po.GetDetails().ToList().Select(x => new PurchaseOrderDetail()
            {
                Category = x.Category,
                IsInventoryControlled = x.IsInventoryControlled,
                Item = x.Item,
                PurchaseOrder = copy,
                Quantity = x.Quantity,
                ToInventoryDate = null,
                Unit = x.Unit,
                UnitPrice = x.UnitPrice
            }).ToList();

            DA.Current.Insert(details);

            TrackingUtility.Track(TrackingCheckpoints.DraftCreated, copy, CacheManager.Current.CurrentUser.ClientID);

            return copy;
        }

        public static bool Update(PurchaseOrder po, Dictionary<string, object> args, ref string errmsg)
        {
            try
            {
                if (args.ContainsKey("AccountID")) po.AccountID = Convert.ToInt32(args["AccountID"]);
                if (args.ContainsKey("ApproverID")) po.Approver = DA.Current.Single<Client>(Convert.ToInt32(args["ApproverID"]));
                if (args.ContainsKey("NeededDate")) po.NeededDate = Convert.ToDateTime(args["NeededDate"]);
                if (args.ContainsKey("Oversized")) po.Oversized = Convert.ToBoolean(args["Oversized"]);
                if (args.ContainsKey("ShippingMethodID")) po.ShippingMethod = DA.Current.Single<ShippingMethod>(Convert.ToInt32(args["ShippingMethodID"]));
                if (args.ContainsKey("Notes")) po.Notes = args["Notes"].ToString();
                if (args.ContainsKey("Attention")) po.Attention = Convert.ToBoolean(args["Attention"]);
                return true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        public static bool SendForApproval(PurchaseOrder po, ref string errmsg)
        {
            try
            {
                po.Status = Status.AwaitingApproval;
                TrackingUtility.Track(TrackingCheckpoints.SentForApproval, po, CacheManager.Current.CurrentUser.ClientID);
                return true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        public static string CleanString(string input)
        {
            string result = input;

            //Remove any punctuation
            string punctuation = @"!@#$%^&*()-_=+[{]}\|;:'"",<.>/?";
            foreach (char c in punctuation.ToCharArray())
            {
                result = result.Replace(c.ToString(), string.Empty);
            }

            //Remove any spaces
            result = result.Replace(" ", string.Empty);

            //Force upper case
            result = result.ToUpper();

            return result;
        }

        public static IList<PurchaseOrder> SelectDraft(int clientId)
        {
            IList<PurchaseOrder> result = DA.Current.Query<PurchaseOrder>().Where(x => x.Client.ClientID == clientId && x.Status == Status.Draft).ToArray();
            return result;
        }

        public static bool DeleteDraft(int poid)
        {
            PurchaseOrder po = DA.Current.Single<PurchaseOrder>(poid);
            if (po.Status == Status.Draft)
            {
                TrackingUtility.Track(TrackingCheckpoints.Deleted, po, CacheManager.Current.CurrentUser.ClientID);
                DA.Current.Delete(po.GetDetails().ToList());
                DA.Current.Delete(po);
                return true;
            }
            return false;
        }

        public static void SaveRealPO(int poid, string reqNum, string realPO, string purchNotes)
        {
            string currentRealPO;
            bool currentIsOrdered;

            PurchaseOrder po = DA.Current.Single<PurchaseOrder>(poid);

            if (po == null)
                throw new Exception(string.Format("Cannot find PurchaseOrder with POID = {0}", poid));

            currentRealPO = po.RealPO;
            currentIsOrdered = po.Status == Status.Ordered;

            TrackingCheckpoints checkpoint = 0;
            bool track = false;

            if (currentIsOrdered)
            {
                checkpoint = TrackingCheckpoints.Modified;
                track = currentIsOrdered && currentRealPO != realPO;
            }
            else
            {
                if (!string.IsNullOrEmpty(realPO))
                {
                    po.Status = Status.Ordered;
                    checkpoint = TrackingCheckpoints.Ordered;
                    track = true;
                }
            }

            string realPoValue = null;

            if (string.IsNullOrEmpty(realPO))
            {
                if (currentIsOrdered)
                {
                    // for already ordered PO do not allow saving an empty RealPO value
                    realPoValue = currentRealPO;
                }
            }
            else
            {
                realPoValue = realPO;
            }

            po.ReqNum = reqNum;
            po.RealPO = realPoValue;
            po.PurchaserNotes = purchNotes;

            if (track)
            {
                // only track if order status set (TrackingCheckpoints.Ordered) or the PO has already been ordered and the RealPO is changing (TrackingCheckpoints.Modified)
                TrackingUtility.Track(checkpoint, po, CacheManager.Current.ClientID);
            }
        }

        public static void CancelPO(int poid)
        {
            PurchaseOrder po = DA.Current.Single<PurchaseOrder>(poid);

            if (po == null)
                throw new Exception(string.Format("Cannot find PurchaseOrder with POID = {0}", poid));

            po.Status = Status.Cancelled;

            TrackingUtility.Track(TrackingCheckpoints.Cancelled, po, CacheManager.Current.ClientID);
        }

        public static double Total(this PurchaseOrder po)
        {
            if (po == null) return 0;

            return po.GetDetails().ToList().Sum(i => i.Quantity * i.UnitPrice);
        }

        public static string ShortCode(this PurchaseOrder po)
        {
            if (po == null) return string.Empty;

            Account acct = po.GetAccount();
            if (acct != null)
                return acct.ShortCode;
            else
                return string.Empty;
        }

        public static IList<PurchaserSearch> PurchasingSelect(DateTime? sd, DateTime? ed, ClaimStatus claimStatus, int purchaserClientId, int creatorClientId, int poid, string realPO, int[] statusId, OrderStatus orderStatus)
        {
            // claimStatus:
            //      All = 0
            //      Unclaimed = 1
            //      Claimed = 2
            //      Claimedby = 3

            // orderStatus:
            //      NotSpecified = 0
            //      Unordered = 1
            //      Ordered = 2

            var query = DA.Current.Query<PurchaserSearch>();

            if (statusId != null && statusId.Length > 0)
                query = query.Where(x => statusId.Contains(x.StatusID));

            if (sd.HasValue)
                query = query.Where(x => x.CreatedDate >= sd.Value);

            if (ed.HasValue)
                query = query.Where(x => x.CreatedDate < ed.Value);

            switch (claimStatus)
            {
                case ClaimStatus.Unclaimed:
                    query = query.Where(x => !x.PurchaserID.HasValue);
                    break;
                case ClaimStatus.Claimed:
                    query = query.Where(x => x.PurchaserID.HasValue);
                    break;
                case ClaimStatus.ClaimedBy:
                    query = query.Where(x => x.PurchaserID == purchaserClientId);
                    break;
            }

            switch (orderStatus)
            {
                case OrderStatus.Unordered:
                    query = query.Where(x => (x.RealPO == null || x.RealPO == ""));
                    break;
                case OrderStatus.Ordered:
                    query = query.Where(x => (x.RealPO != null && x.RealPO != ""));
                    break;
            }

            if (purchaserClientId > 0 && claimStatus == ClaimStatus.All)
                query = query.Where(x => x.PurchaserID == purchaserClientId);

            if (creatorClientId > 0)
                query = query.Where(x => x.ClientID == creatorClientId);

            if (poid > 0)
                query = query.Where(x => x.POID == poid);

            if (!string.IsNullOrEmpty(realPO))
                query = query.Where(x => x.RealPO.Contains(realPO));

            var result = query.ToList();

            return result;
        }

        public static DataTable CreatePurchaserSearchDataTable(IList<PurchaserSearch> query)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("POID", typeof(int));
            dt.Columns.Add("StatusID", typeof(int));
            dt.Columns.Add("CreatedDate", typeof(DateTime));
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("DisplayName", typeof(string));
            dt.Columns.Add("PurchaserID", typeof(int));
            dt.Columns.Add("PurchaserDisplayName", typeof(string));
            dt.Columns.Add("Total", typeof(double));
            dt.Columns.Add("RealPO", typeof(string));

            foreach (var item in query)
            {
                var ndr = dt.NewRow();
                ndr.SetField("POID", item.POID);
                ndr.SetField("StatusID", item.StatusID);
                ndr.SetField("CreatedDate", item.CreatedDate);
                ndr.SetField("ClientID", item.ClientID);
                ndr.SetField("DisplayName", item.DisplayName);
                ndr.SetField("PurchaserID", item.PurchaserID.GetValueOrDefault());
                ndr.SetField("PurchaserDisplayName", item.PurchaserDisplayName);
                ndr.SetField("Total", item.Total);
                ndr.SetField("RealPO", item.RealPO);
                dt.Rows.Add(ndr);
            }

            return dt;
        }

        public static double GetTotal(int poid)
        {
            var details = DA.Current.Query<PurchaseOrderDetail>().Where(x => x.PurchaseOrder.POID == poid);
            // The nullable cast will allow for cases where a PO has no details
            var result = details.Sum(x => (double?)(x.Quantity * x.UnitPrice)).GetValueOrDefault();
            return result;
        }
    }
}
