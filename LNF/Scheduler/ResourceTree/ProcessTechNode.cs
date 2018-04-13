using System.Collections.Generic;

namespace LNF.Scheduler.ResourceTree
{
    public class ProcessTechNode : IResourceTreeNode
    {
        public int LabID { get; set; }
        public int ProcessTechID { get; set; }
        public string ProcessTechName { get; set; }
        public string ProcessTypeDescription { get; set; }
        public bool Expanded { get; set; }
        public IEnumerable<ResourceNode> Resources { get; set; }
    }
}
