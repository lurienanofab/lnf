namespace LNF.Billing
{
    public class InvoiceLineItem
    {
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public double Cost { get; set; }

        public double LineTotal
        {
            get { return Quantity * Cost; }
        }
    }
}
