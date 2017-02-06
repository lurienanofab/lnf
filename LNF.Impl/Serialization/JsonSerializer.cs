using System;

namespace LNF.Impl.Serialization
{
    public class JsonSerializer : ISerializer
    {
        public string SerializeObject(object obj)
        {
            string result = SerializationUtility.Serialize(obj, new NHibernateContractResovler());
            return result;
        }

        public string Serialize<T>(T obj, params string[] properties) where T : new()
        {
            string result;
            if (properties.Length > 0)
                result = SerializationUtility.Serialize(obj, new DynamicContractResolver<T>(properties));
            else
                result = SerializationUtility.Serialize(obj);
            return result;
        }

        public T Deserialize<T>(string value)
        {
            T result = SerializationUtility.Deserialize<T>(value);
            return result;
        }

        public T DeserializeAnonymous<T>(string value, T anonymousObject)
        {
            T result = SerializationUtility.DeserializeAnonymous(value, anonymousObject);
            return result;
        }

        public object DeserializeObject(string value, Type t)
        {
            object result = SerializationUtility.DeserializeObject(value, t);
            return result;
        }
    }
}
