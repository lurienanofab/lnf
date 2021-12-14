using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using LNF.Scheduler;

namespace LNF.Impl.Scheduler
{
    public class KioskRedirects : IKioskRedirects
    {
        [JsonProperty("default")]
        public string Default { get; set; }

        [JsonProperty("items"), JsonConverter(typeof(MyJsonConverter))]
        public IEnumerable<IKioskRedirectItem> Items { get; set; }
    }

    public class MyJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<IKioskRedirectItem>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<IEnumerable<KioskRedirectItem>>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
