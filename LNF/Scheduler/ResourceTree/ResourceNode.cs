using System;

namespace LNF.Scheduler.ResourceTree
{
    public class ResourceNode : IResourceTreeNode
    {
        public int ProcessTechID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public bool Expanded { get; set; }
    }
}
