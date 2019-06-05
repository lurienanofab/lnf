namespace LNF.Models.Scheduler
{
    public interface IResourceCost
    {
        int ResourceID { get; }
        int ChargeTypeID { get; }
        string AcctPer { get; }
        decimal AddVal { get; }
        decimal MulVal { get; }
        decimal OvertimeMultiplier { get; }

        decimal BookingFeeMultiplier();
        decimal BookingFeeRate();
        decimal HourlyRate();
        decimal OverTimeRate();
        decimal PerUseRate();
    }
}