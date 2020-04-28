using System;

namespace LNF.Util.Serialization
{
    public interface ISerializer
    {
        string SerializeObject(object obj);
        string Serialize<T>(T obj, params string[] properties) where T : new();
        T Deserialize<T>(string value);
        T DeserializeAnonymous<T>(string value, T anonymousObject);
        object DeserializeObject(string value, Type t);
    }
}
