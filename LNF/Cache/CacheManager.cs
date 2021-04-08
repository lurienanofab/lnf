using System;

namespace LNF.Cache
{
    public static class CacheManager
    {
        public static ICache Current { get; private set; }

        public static void Setup(ICache cache)
        {
            var stack = new System.Diagnostics.StackTrace();

            if (Current == null)
            {
                Current = cache;
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] CacheManager setup complete.{Environment.NewLine}{stack}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] CacheManager has already been setup. What are you trying to do?{Environment.NewLine}{stack}");
            }
        }
    }
}
