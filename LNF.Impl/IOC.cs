using FluentNHibernate.Cfg;
using SimpleInjector;

namespace LNF.Impl
{
    public static class IOC
    {
        static IOC()
        {
            Container = new Container();
            Container.Register(typeof(FluentConfiguration), () => SessionConfiguration.GetConfiguration(), Lifestyle.Singleton);
        }

        public static Container Container { get; }
    }
}
