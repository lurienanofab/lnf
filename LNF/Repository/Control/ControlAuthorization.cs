using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Control
{
    public class ControlAuthorization:IDataItem
    {
        public virtual int ActionID { get; set; }
        public virtual int ActionInstanceID { get; set; }
        public virtual int ClientID {get;set;}
        public virtual string Location { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else
                return obj.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            return string.Format("{0}-{1}-{2}-{3}", ActionID, ActionInstanceID, ClientID, Location).GetHashCode();
        }
    }
}
