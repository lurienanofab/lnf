using System.Collections.Generic;

namespace LNF.Scheduler.ResourceTree
{
    public class BuildingNode : IResourceTreeNode
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public string BuildingDescription { get; set; }
        public bool Expanded { get; set; }
        public IEnumerable<LabNode> Labs { get; set; }
    }
}
