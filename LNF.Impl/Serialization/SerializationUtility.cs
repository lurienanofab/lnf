using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace LNF.Impl.Serialization
{
    public static class SerializationUtility
    {
        static SerializationUtility()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new TimeSpanConverter());
                settings.ContractResolver = new NHibernateContractResovler();
                settings.Formatting = Formatting.None;
                return settings;
            };
        }

        public static string Serialize(object obj)
        {
            string result = JsonConvert.SerializeObject(obj);
            return result;
        }

        public static string Serialize(object obj, IContractResolver resolver)
        {
            var settings = new JsonSerializerSettings() { ContractResolver = resolver };
            string result = JsonConvert.SerializeObject(obj, Formatting.None, settings);
            return result;
        }

        public static T Deserialize<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                return default(T);
            T result = JsonConvert.DeserializeObject<T>(value);
            return result;
        }

        public static T DeserializeAnonymous<T>(string value, T anonymousObject)
        {
            T result = JsonConvert.DeserializeAnonymousType(value, anonymousObject);
            return result;
        }

        public static object DeserializeObject(string value, Type t)
        {
            try
            {
                object result = JsonConvert.DeserializeObject(value, t, new JsonSerializerSettings());
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
