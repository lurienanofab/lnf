using System;

namespace LNF.Models.Data
{
    public interface ICost
    {
        int CostID { get; set; }
        int ChargeTypeID { get; set; }
        string ChargeTypeName { get; set; }
        string TableNameOrDescription { get; set; }
        int RecordID { get; set; }
        string AcctPer { get; set; }
        decimal AddVal { get; set; }
        decimal MulVal { get; set; }
        DateTime EffDate { get; set; }
        DateTime CreatedDate { get; set; }
    }
}