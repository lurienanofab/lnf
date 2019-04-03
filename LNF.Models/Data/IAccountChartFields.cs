namespace LNF.Models.Data
{
    public interface IAccountChartFields
    {
        int AccountID { get; }
        string AccountName { get; }
        string Number { get; }
        string Account { get; }
        string Fund { get; }
        string Department { get; }
        string Program { get; }
        string Class { get; }
        string Project { get; }
        string ShortCode { get; }
    }
}