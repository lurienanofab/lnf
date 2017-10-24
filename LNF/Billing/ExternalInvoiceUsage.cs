using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public class ExternalInvoiceUsage : IEnumerable<ExternalInvoiceLineItem>
    {
        private List<ExternalInvoiceLineItem> _items;

        public ExternalInvoiceUsage()
        {
            _items = new List<ExternalInvoiceLineItem>();
        }

        public ExternalInvoiceUsage(IEnumerable<ExternalInvoiceLineItem> items)
        {
            _items = items.ToList();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public decimal TotalCharges
        {
            get
            {
                double lineTotal = _items.Sum(x => x.LineTotal);
                return Convert.ToDecimal(lineTotal);
            }
        }

        public void Add(ExternalInvoiceLineItem item)
        {
            _items.Add(item);
        }

        public double GetChargeTypeTotal(int chargeTypeId)
        {
            return _items.Where(x => x.ChargeTypeID == chargeTypeId).Sum(x => x.LineTotal);
        }

        public IEnumerable<ExternalInvoiceLineItem> GetAccountItems(int accountId)
        {
            return _items.Where(x => x.AccountID == accountId);
        }

        public IEnumerator<ExternalInvoiceLineItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
