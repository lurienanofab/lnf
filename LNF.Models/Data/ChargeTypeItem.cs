namespace LNF.Models.Data
{
    public class ChargeTypeItem : IChargeType
    {
        public int ChargeTypeID { get; set; }
        public string ChargeTypeName { get; set; }
        public int AccountID { get; set; }
        public bool IsInternal { get; set; }
    }
}
