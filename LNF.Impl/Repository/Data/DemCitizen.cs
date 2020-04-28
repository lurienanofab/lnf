using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class DemCitizen : IDataItem
    {
        public virtual int DemCitizenID { get; set; }
        public virtual string DemCitizenValue { get; set; }
    }
}
