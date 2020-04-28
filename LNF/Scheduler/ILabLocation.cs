namespace LNF.Scheduler
{
    public interface ILabLocation
    {
        int LabLocationID { get; set; }
        int LabID { get; set; }
        string LocationName { get; set; }
    }
}
