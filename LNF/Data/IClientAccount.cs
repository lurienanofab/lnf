namespace LNF.Data
{
    public interface IClientAccount : IClient, IAccount
    {
        int ClientAccountID { get; set; }
        bool IsDefault { get; set; }
        bool Manager { get; set; }
        bool ClientAccountActive { get; set; }
    }
}