using System.Reflection;
using System.Runtime.Caching;

namespace LNF.Cache
{
    //https://stackoverflow.com/questions/22392634/how-to-measure-current-size-of-net-memory-cache-4-0

    public static class MemoryCacheHackExtenstions
    {
        public static long GetApproximateSize(this MemoryCache cache)
        {
            try
            {
                var statsField = typeof(MemoryCache).GetField("_stats", BindingFlags.NonPublic | BindingFlags.Instance);
                var statsValue = statsField.GetValue(cache);
                var monitorField = statsValue.GetType().GetField("_cacheMemoryMonitor", BindingFlags.NonPublic | BindingFlags.Instance);
                var monitorValue = monitorField.GetValue(statsValue);
                var sizeField = monitorValue.GetType().GetField("_sizedRefMultiple", BindingFlags.NonPublic | BindingFlags.Instance);
                var sizeValue = sizeField.GetValue(monitorValue);
                var approxProp = sizeValue.GetType().GetProperty("ApproximateSize", BindingFlags.NonPublic | BindingFlags.Instance);
                return (long)approxProp.GetValue(sizeValue, null);
            }
            catch
            {
                return -1;
            }
        }
    }
}
