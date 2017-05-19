using System;

namespace LNF.Repository.Reporting
{
    public class ClientEmailPreference : IDataItem
    {
        public virtual int ClientEmailPreferenceID { get; set; }
        public virtual int EmailPreferenceID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual DateTime EnableDate { get; set; }
        public virtual DateTime? DisableDate { get; set; }
    }
}
