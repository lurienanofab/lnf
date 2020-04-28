using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class DemDisability : IDataItem
    {
        public virtual int DemDisabilityID { get; set; }
        public virtual string DemDisabilityValue { get; set; }
    }
}
