namespace LNF.Scheduler
{
    public interface IProcessInfo
    {
        int ProcessInfoID { get; set; }
        int ResourceID { get; set; }
        string ProcessInfoName { get; set; }
        string ParamName { get; set; }
        string ValueName { get; set; }
        string Special { get; set; }
        bool AllowNone { get; set; }
        int Order { get; set; }
        bool RequireValue { get; set; }
        bool RequireSelection { get; set; }
        int MaxAllowed { get; set; }
    }
}