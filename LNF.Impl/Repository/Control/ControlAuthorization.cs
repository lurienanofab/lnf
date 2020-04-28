using LNF.Control;
using LNF.DataAccess;

namespace LNF.Impl.Repository.Control
{
    public class ControlAuthorization : IControlAuthorization, IDataItem
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
                return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return new { ActionID, ActionInstanceID, ClientID, Location }.GetHashCode();
        }
    }
}
