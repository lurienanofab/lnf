using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Data
{
    public class AddressItem
    {
        private Address addr;

        public AddressItem()
        {
            addr = null;
            Deleted = false;
        }

        public static AddressItem Create(string addressType, Address address){
            AddressItem result = new AddressItem();
            result.AddressType = addressType;
            result.Load(address);
            return result;
        }

        public string AddressType { get; set; }
        public int AddressID { get; set; }
        public string AttentionLine { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool Deleted { get; set; }

        public void Load(Address address)
        {
            addr = address;
            AddressID = addr.AddressID;
            AttentionLine = addr.InternalAddress;
            Street1 = addr.StrAddress1;
            Street2 = addr.StrAddress2;
            City = addr.City;
            State = addr.State;
            Zip = addr.Zip;
            Country = addr.Country;
            Deleted = false;
        }

        public string GetAddressType()
        {
            switch (AddressType)
            {
                case "billing":
                    return "Billing";
                case "shipping":
                    return "Shipping";
                case "client":
                    return "Client";
                default:
                    return "[unknown]";
            }
        }
    }
}
