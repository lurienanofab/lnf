namespace LNF.Scheduler
{
    public interface IProcessInfoLine
    {
        int ProcessInfoLineID { get; set; }
        int ProcessInfoID { get; set; }
        int ProcessInfoLineParamID { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        string Param { get; set; }
        string ParameterName { get; set; }
        int ParameterType { get; set; }
        double MinValue { get; set; }
        double MaxValue { get; set; }
    }
}