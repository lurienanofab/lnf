namespace LNF.Data
{
    public interface IAccountNumber
    {
        string Account { get; }
        string Class { get; }
        string Department { get; }
        string Fund { get; }
        string Program { get; }
        string Project { get; }
        string Value { get; }
    }
}