using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class DemRace : IDataItem
    {
        public virtual int DemRaceID { get; set; }
        public virtual string DemRaceValue { get; set; }
    }
}
