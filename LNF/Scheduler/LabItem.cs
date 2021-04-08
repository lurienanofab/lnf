namespace LNF.Scheduler
{
    public class LabItem : BuildingItem, ILab
    {
        public int LabID { get; set; }
        public string LabName { get; set; }
        public string LabDisplayName { get; set; }
        public string LabDescription { get; set; }
        public bool LabIsActive { get; set; }
        public override string ToString() => $"{LabDisplayName} [{LabID}]";
    }
}
