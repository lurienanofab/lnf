using StructureMap;

namespace LNF.Tests
{
    public static class IOC
    {
        public static IContainer Container { get; }

        static IOC()
        {
            Container = new Container(cfg =>
            {
                cfg.Scan(s =>
                {
                    s.AssembliesFromApplicationBaseDirectory();
                    s.WithDefaultConventions();
                });
            });
        }
    }
}
