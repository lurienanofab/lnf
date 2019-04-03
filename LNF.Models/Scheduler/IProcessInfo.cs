namespace LNF.Models.Scheduler
{
    public interface IProcessInfo
    {
        bool AllowNone { get; set; }
        int Order { get; set; }
        string ParamName { get; set; }
        int ProcessInfoID { get; set; }
        string ProcessInfoName { get; set; }
        bool RequireSelection { get; set; }
        bool RequireValue { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        string Special { get; set; }
        string ValueName { get; set; }
    }
}