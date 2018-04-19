namespace LNF
{
    public interface IDependencyResolver
    {
        T GetInstance<T>();
        void BuildUp(object target);
    }
}
