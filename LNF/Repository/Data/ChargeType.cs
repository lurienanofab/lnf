namespace LNF.Repository.Data
{
    public class ChargeType : IDataItem
    {
        public virtual int ChargeTypeID { get; set; }
        public virtual string ChargeTypeName { get; set; }
        public virtual int AccountID { get; set; }
    }
}
