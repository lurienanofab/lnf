using LNF.Billing.Process;

namespace LNF.Billing
{
    public class UpdateClientBillingResult
    {
        public WriteToolDataCleanResult WriteToolDataCleanProcessResult { get; set; }
        public WriteRoomDataCleanResult WriteRoomDataCleanProcessResult { get; set; }
        public WriteToolDataResult WriteToolDataProcessResult { get; set; }
        public WriteRoomDataResult WriteRoomDataProcessResult { get; set; }
        public PopulateToolBillingResult PopulateToolBillingProcessResult { get; set; }
        public PopulateRoomBillingResult PopulateRoomBillingProcessResult { get; set; }
        public PopulateSubsidyBillingResult PopulateSubsidyBillingProcessResult { get; set; }
    }
}
