using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class DemGender : IDataItem
    {
        public virtual int DemGenderID { get; set; }
        public virtual string DemGenderValue { get; set; }
    }
}
