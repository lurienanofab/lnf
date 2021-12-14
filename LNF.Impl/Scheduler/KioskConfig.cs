using Newtonsoft.Json;
using System.IO;
using System.Linq;
using LNF.Scheduler;
using System;

namespace LNF.Impl.Scheduler
{
    public class KioskConfig : IKioskConfig
    {
        [JsonProperty("redirects"), JsonConverter(typeof(KioskRedirectsConverter))]
        public IKioskRedirects Redirects { get; set; }

        public string GetRedirectUrl(string ipaddr)
        {
            var item = Redirects.Items.FirstOrDefault(x => x.IP == ipaddr);

            if (item == null)
                return Redirects.Default;
            else
                return item.URL;
        }
    }

    public class KioskRedirectsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IKioskRedirects).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<KioskRedirects>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
