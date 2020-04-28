namespace LNF.Data
{
    public interface IChargeType
    {
        int ChargeTypeID { get; set; }
        string ChargeTypeName { get; set; }
        int AccountID { get; set; }
        bool IsInternal { get; set; }
    }
}
