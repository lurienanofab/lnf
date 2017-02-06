using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace LNF.Cache
{
    public static class CacheObjectFactory
    {
        public static CacheObject<T> CreateOne<T>(T value)
        {
            return new CacheObject<T>() { Value = value };
        }

        public static IEnumerable<CacheObject<T>> CreateMany<T>(IEnumerable<T> values)
        {
            foreach (T value in values)
            {
                yield return CreateOne(value);
            }
        }
    }

    public class CacheObject<T>
    {
        public CacheObject()
        {
            CreatedAt = DateTime.Now;
        }

        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id {get;set;}

        public T Value { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
