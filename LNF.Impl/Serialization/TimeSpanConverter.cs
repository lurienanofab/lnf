using Newtonsoft.Json;
using System;

namespace LNF.Impl.Serialization
{
    public class TimeSpanConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(TimeSpan).IsAssignableFrom(objectType);
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                return TimeSpan.Zero;

            TimeSpan ts;
            if (!TimeSpan.TryParse(reader.Value.ToString(), out ts))
                return TimeSpan.Zero;

            return ts;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
