namespace LNF
{
    public interface IDependencyResolver
    {
        T GetInstance<T>();
    }
}
