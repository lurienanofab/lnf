namespace LNF.Ordering
{
    public class VendorItem : IVendor
    {
        public int VendorID { get; set; }
        public int ClientID { get; set; }
        public string VendorName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string URL { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
    }
}
