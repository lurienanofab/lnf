namespace LNF.Data
{
    public class AddressItem : IAddress
    {
        public int AddressID { get; set; }
        public string InternalAddress { get; set; }
        public string StrAddress1 { get; set; }
        public string StrAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
}
