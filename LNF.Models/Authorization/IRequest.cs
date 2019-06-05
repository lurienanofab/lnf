namespace LNF.Models.Authorization
{
    public interface IRequest
    {
        void AddParameter(string name, object value);
    }
}
