using Newtonsoft.Json;
using LNF.Scheduler;

namespace LNF.Impl.Scheduler
{
    public class KioskRedirectItem : IKioskRedirectItem
    {
        [JsonProperty("ip")]
        public string IP { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; }
    }
}
