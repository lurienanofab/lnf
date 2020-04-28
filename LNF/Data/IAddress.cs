namespace LNF.Data
{
    public interface IAddress
    {
        int AddressID { get; set; }
        string InternalAddress { get; set; }
        string StrAddress1 { get; set; }
        string StrAddress2 { get; set; }
        string City { get; set; }
        string State { get; set; }
        string Zip { get; set; }
        string Country { get; set; }
    }
}
