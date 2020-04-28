namespace LNF.Data
{
    public class OrgItem : IOrg
    {
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public int DefClientAddressID { get; set; }
        public int DefBillAddressID { get; set; }
        public int DefShipAddressID { get; set; }
        public bool NNINOrg { get; set; }
        public bool PrimaryOrg { get; set; }
        public bool OrgActive { get; set; }
        public int OrgTypeID { get; set; }
        public string OrgTypeName { get; set; }
        public int ChargeTypeID { get; set; }
        public string ChargeTypeName { get; set; }
        public int ChargeTypeAccountID { get; set; }
    }
}
