namespace LNF.Models.Reporting
{
    public class ReportingService : IReportingService
    {
        public IClientItemManager ClientItem { get; }

        public ReportingService(IClientItemManager clientItem)
        {
            ClientItem = clientItem;
        }
    }
}
