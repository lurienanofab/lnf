namespace LNF.Models.Scheduler
{
    public class ResourceCostModel
    {
        public int ResourceID { get; set; }
        public int ChargeTypeID { get; set; }
        public string ChargeTypeName { get; set; }
        public decimal AddVal { get; set; }
        public decimal MulVal { get; set; }
    }
}
