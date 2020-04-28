namespace LNF.Ordering
{
    public interface IVendor
    {
        int VendorID { get; set; }
        int ClientID { get; set; }
        string VendorName { get; set; }
        string Address1 { get; set; }
        string Address2 { get; set; }
        string Address3 { get; set; }
        string Contact { get; set; }
        string Phone { get; set; }
        string Fax { get; set; }
        string URL { get; set; }
        string Email { get; set; }
        bool Active { get; set; }
    }
}
