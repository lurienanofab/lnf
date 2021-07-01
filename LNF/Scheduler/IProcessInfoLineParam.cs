namespace LNF.Scheduler
{
    public interface IProcessInfoLineParam
    {
        int ProcessInfoLineParamID { get; set; }
        int ResourceID { get; set; }
        string ParameterName { get; set; }
        string ParameterUnit { get; set; }
        int ParameterType { get; set; }
    }
}
