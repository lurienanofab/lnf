namespace LNF.Data
{
    public interface IOrg
    {
        int OrgID { get; set; }
        string OrgName { get; set; }
        int DefClientAddressID { get; set; }
        int DefBillAddressID { get; set; }
        int DefShipAddressID { get; set; }
        bool NNINOrg { get; set; }
        bool PrimaryOrg { get; set; }
        bool OrgActive { get; set; }
        int OrgTypeID { get; set; }
        string OrgTypeName { get; set; }
        int ChargeTypeID { get; set; }
        string ChargeTypeName { get; set; }
        int ChargeTypeAccountID { get; set; }
    }
}
