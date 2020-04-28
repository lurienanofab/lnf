using LNF.Util.Serialization;
using System;

namespace LNF.Impl.Util.Serialization
{
    public class JsonSerializer : ISerializer
    {
        public string SerializeObject(object obj)
        {
            string result = NewtonsoftJsonHelper.Serialize(obj, new NHibernateContractResovler());
            return result;
        }

        public string Serialize<T>(T obj, params string[] properties) where T : new()
        {
            string result;
            if (properties.Length > 0)
                result = NewtonsoftJsonHelper.Serialize(obj, new DynamicContractResolver<T>(properties));
            else
                result = NewtonsoftJsonHelper.Serialize(obj);
            return result;
        }

        public T Deserialize<T>(string value)
        {
            T result = NewtonsoftJsonHelper.Deserialize<T>(value);
            return result;
        }

        public T DeserializeAnonymous<T>(string value, T anonymousObject)
        {
            T result = NewtonsoftJsonHelper.DeserializeAnonymous(value, anonymousObject);
            return result;
        }

        public object DeserializeObject(string value, Type t)
        {
            object result = NewtonsoftJsonHelper.DeserializeObject(value, t);
            return result;
        }
    }
}
