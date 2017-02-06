using System;
using System.Collections.Generic; 
using System.Text; 

namespace LNF.Repository.Data
{    
    public class Address : IDataItem
    {
        public virtual int AddressID { get; set; }
        public virtual string InternalAddress { get; set; }
        public virtual string StrAddress1 { get; set; }
        public virtual string StrAddress2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string Country { get; set; }
    }
}
