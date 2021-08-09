using MongoDB.Bson.Serialization;
using System;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;

namespace LNF.Impl.Mongo
{
    public static class MongoUtility
    {
        public static void RegisterClassMaps<TClass, TMember>(Expression<Func<TClass, TMember>> idExpression)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(TClass)))
            {
                BsonClassMap.RegisterClassMap<TClass>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(idExpression).SetSerializer(new GuidSerializer(BsonType.String));
                });
            }
        }
    }
}
