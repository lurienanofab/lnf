using LNF.Models.Billing.Process;

namespace LNF.Models.Billing
{
    public class UpdateClientBillingResult
    {
        public WriteToolDataCleanProcessResult WriteToolDataCleanProcessResult { get; set; }
        public WriteRoomDataCleanProcessResult WriteRoomDataCleanProcessResult { get; set; }
        public WriteToolDataProcessResult WriteToolDataProcessResult { get; set; }
        public WriteRoomDataProcessResult WriteRoomDataProcessResult { get; set; }
        public PopulateToolBillingProcessResult PopulateToolBillingProcessResult { get; set; }
        public PopulateRoomBillingProcessResult PopulateRoomBillingProcessResult { get; set; }
        public PopulateSubsidyBillingProcessResult PopulateSubsidyBillingProcessResult { get; set; }
    }
}
