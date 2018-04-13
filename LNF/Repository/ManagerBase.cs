namespace LNF.Repository
{
    public abstract class ManagerBase : IManager
    {
        public ISession Session { get; }

        public ManagerBase(ISession session)
        {
            Session = session;
        }
    }
}
