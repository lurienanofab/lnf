using System.Collections.Generic;

namespace LNF.Scheduler.ResourceTree
{
    public class LabNode : IResourceTreeNode
    {
        public int BuildingID { get; set; }
        public int LabID { get; set; }
        public string LabName { get; set; }
        public string LabDescription { get; set; }
        public bool Expanded { get; set; }
        public IEnumerable<ProcessTechNode> ProcessTechs { get; set; }
    }
}
