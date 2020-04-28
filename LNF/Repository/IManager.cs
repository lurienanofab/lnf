using LNF.DataAccess;

namespace LNF.Repository
{
    public interface IManager
    {
        ISession Session { get; }
    }
}
