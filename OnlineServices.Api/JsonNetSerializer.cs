using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace OnlineServices.Api
{
    public class JsonNetSerializer : ISerializer
    {
        public static JsonNetSerializer Default { get; }

        static JsonNetSerializer()
        {
            Default = new JsonNetSerializer();
        }

        public string ContentType { get; set; }

        private JsonNetSerializer()
        {
            ContentType = "application/json";
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }

    public class JsonNetDeserializer : IDeserializer
    {
        public static JsonNetDeserializer Default { get; }

        static JsonNetDeserializer()
        {
            Default = new JsonNetDeserializer();
        }

        private JsonNetDeserializer() { }

        public T Deserialize<T>(IRestResponse response)
        {
            var result =  JsonConvert.DeserializeObject<T>(response.Content);
            return result;
        }
    }
}
