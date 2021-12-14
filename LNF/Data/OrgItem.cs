using System.Runtime.Serialization;

namespace LNF.Data
{
    [DataContract]
    [KnownType(typeof(OrgItem))]
    public class OrgItem : IOrg
    {
        [DataMember] public int OrgID { get; set; }
        [DataMember] public string OrgName { get; set; }
        [DataMember] public int DefClientAddressID { get; set; }
        [DataMember] public int DefBillAddressID { get; set; }
        [DataMember] public int DefShipAddressID { get; set; }
        [DataMember] public bool NNINOrg { get; set; }
        [DataMember] public bool PrimaryOrg { get; set; }
        [DataMember] public bool OrgActive { get; set; }
        [DataMember] public int OrgTypeID { get; set; }
        [DataMember] public string OrgTypeName { get; set; }
        [DataMember] public int ChargeTypeID { get; set; }
        [DataMember] public string ChargeTypeName { get; set; }
        [DataMember] public int ChargeTypeAccountID { get; set; }
    }
}
