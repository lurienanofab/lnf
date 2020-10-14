namespace LNF.Scheduler
{
    public interface IProcessInfoLine
    {
        int ProcessInfoLineID { get; set; }
        int ProcessInfoID { get; set; }
        string Param { get; set; }
        double MinValue { get; set; }
        double MaxValue { get; set; }
        int ProcessInfoLineParamID { get; set; }
    }
}