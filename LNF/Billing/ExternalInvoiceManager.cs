using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Billing
{
    public class ExternalInvoiceManager : IEnumerable<KeyValuePair<string, ExternalInvoiceUsage>>
    {
        public int AccountID { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public bool ShowRemote { get; }
        public bool IncludeAccountsWithNoUsage { get; set; }

        private IDictionary<string, ExternalInvoiceUsage> _data;

        /// <summary>
        /// Generates invoices for all accounts.
        /// </summary>
        public ExternalInvoiceManager(DateTime sd, DateTime ed, bool showRemote)
            : this(0, sd, ed, showRemote) { }

        /// <summary>
        /// Generates invoices for a single account.
        /// </summary>
        public ExternalInvoiceManager(int accountId, DateTime sd, DateTime ed, bool showRemote)
        {
            AccountID = 0;
            StartDate = sd;
            EndDate = ed;
            ShowRemote = showRemote;
            IncludeAccountsWithNoUsage = false;

            _data = ExternalInvoiceUtility.GetAllUsage(StartDate, EndDate, AccountID, ShowRemote);
        }

        public ExternalInvoiceUsage this[string key]
        {
            get { return _data[key]; }
        }

        public IEnumerable<ExternalInvoiceSummaryItem> GetSummary()
        {
            var chargeTypes = ExternalInvoiceUtility.GetExternalChargeTypes();

            foreach (DataRow dr in chargeTypes.Rows)
            {
                var chargeTypeId = dr.Field<int>("ChargeTypeID");
                var chargeTypeName = dr.Field<string>("ChargeType");

                yield return new ExternalInvoiceSummaryItem()
                {
                    ChargeTypeID = chargeTypeId,
                    ChargeTypeName = chargeTypeName,
                    TotalCharges = _data.Sum(x => x.Value.GetChargeTypeTotal(chargeTypeId))
                };
            }
        }

        public IList<ExternalInvoice> GetInvoices(int orgAcctId = 0)
        {
            var headers = GetHeaders(orgAcctId);
            IList<ExternalInvoice> result = new List<ExternalInvoice>();

            foreach (var h in headers)
            {
                var item = new ExternalInvoice()
                {
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Header = h,
                    Usage = GetAccountUsage(h.AccountID)
                };

                result.Add(item);
            }

            return result;
        }

        public ExternalInvoiceUsage GetAccountUsage(int accountId)
        {
            List<ExternalInvoiceLineItem> result = _data.SelectMany(x => x.Value.GetAccountItems(accountId)).ToList();
            return new ExternalInvoiceUsage(result);
        }

        public IEnumerable<ExternalInvoiceHeader> GetHeaders(int orgAcctId = 0)
        {
            var headers = new List<ExternalInvoiceHeader>();

            var lineItems = _data.SelectMany(x => x.Value).Where(x => orgAcctId == 0 || x.OrgAcctID == orgAcctId);

            // First get orgs based on usage. If one has usage it should be included even if it's not active.
            foreach (var item in lineItems)
            {
                if (!headers.Any(x => x.AccountID == item.AccountID))
                {
                    headers.Add(new ExternalInvoiceHeader()
                    {
                        OrgAcctID = item.OrgAcctID,
                        OrgID = item.OrgID,
                        AccountID = item.AccountID,
                        OrgName = item.OrgName,
                        AccountName = item.AccountName,
                        PoEndDate = item.PoEndDate,
                        PoRemainingFunds = item.PoRemainingFunds,
                        InvoiceNumber = item.InvoiceNumber,
                        DeptRef = item.DeptRef,
                        HasActivity = true
                    });
                }
            }

            if (IncludeAccountsWithNoUsage)
            {
                // Second get orgs based on active during period. Here we will get the orgs that were active but didn't have any usage.

                // get all active external orgs along with their accounts
                var dtActiveOrg = ExternalInvoiceUtility.GetActiveExternalOrgs(StartDate, EndDate);

                foreach (DataRow dr in dtActiveOrg.Rows)
                {
                    if (!headers.Any(x => x.AccountID == dr.Field<int>("AccountID")))
                    {
                        // found an active org with no activity
                        headers.Add(new ExternalInvoiceHeader()
                        {
                            OrgAcctID = dr.Field<int>("OrgAcctID"),
                            OrgID = dr.Field<int>("OrgID"),
                            AccountID = dr.Field<int>("AccountID"),
                            OrgName = dr.Field<string>("OrgName"),
                            AccountName = dr.Field<string>("Name"),
                            PoEndDate = dr.Field<DateTime?>("PoEndDate"),
                            PoRemainingFunds = dr.Field<decimal?>("PoRemainingFunds").GetValueOrDefault(),
                            InvoiceNumber = dr.Field<string>("InvoiceNumber"),
                            DeptRef = dr.Field<string>("DisplayDeptRefID"),
                            HasActivity = false
                        });
                    }
                }
            }

            var result = headers.Where(x => IncludeAccountsWithNoUsage || x.HasActivity).OrderBy(x => x.OrgName).ThenBy(x => x.AccountName);

            return result;
        }

        public IEnumerator<KeyValuePair<string, ExternalInvoiceUsage>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
