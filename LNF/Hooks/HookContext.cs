namespace LNF.Hooks
{
    public abstract class HookContext
    {
        public IProvider Provider { get; }

        internal HookContext(IProvider provider)
        {
            Provider = provider;
        }
    }
}
