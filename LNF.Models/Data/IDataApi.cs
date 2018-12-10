namespace LNF.Models.Data
{
    public interface IDataApi
    {
        IDefaultClient Default { get; }
        IClientClient Client { get; }
    }
}
