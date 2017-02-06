using System;

namespace LNF.Repository.Data
{
    public class BadgeCardClient : IDataItem
    {
        public virtual string BadgeID { get; set; }
        public virtual string CardNumber { get; set; }
        public virtual Client Client { get; set; }
        public virtual string UserName { get; set; }
        public virtual DateTime BadgeExpiration { get; set; }
        public virtual DateTime CardExpiration { get; set; }
        public virtual string CardStatus { get; set; }
    }
}
