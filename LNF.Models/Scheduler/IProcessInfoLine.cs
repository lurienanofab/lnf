namespace LNF.Models.Scheduler
{
    public interface IProcessInfoLine
    {
        double MaxValue { get; set; }
        double MinValue { get; set; }
        string Param { get; set; }
        string ParameterName { get; set; }
        int ParameterType { get; set; }
        int ProcessInfoID { get; set; }
        int ProcessInfoLineID { get; set; }
        int ProcessInfoLineParamID { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
    }
}