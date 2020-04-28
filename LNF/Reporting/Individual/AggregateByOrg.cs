using System.Collections.Generic;

namespace LNF.Reporting.Individual
{
    public class AggregateByOrg
    {
        public IEnumerable<RoomByOrgItem> RoomByOrg { get; set; }
        public IEnumerable<ToolByOrgItem> ToolByOrg { get; set; }
        public IEnumerable<StoreByOrgItem> StoreByOrg { get; set; }
        public IEnumerable<SubsidyByOrgItem> SubsidyByOrg { get; set; }
    }
}