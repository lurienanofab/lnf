using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Impl.Cache
{
    public class MongoCache
    {
        private MongoClient _client;

        public MongoCache()
        {
            //<add key="MongoConnectionString" value="mongodb://localhost" />
            _client = new MongoClient(ConfigurationManager.AppSettings["MongoConnectionString"]);
        }

        public IList<T> Get<T>(Expression<Func<T, bool>> filter)
        {
            string name = typeof(T).Name;

            var col = GetCollection<MongoCacheObject<T>>(name);

            var keys = new IndexKeysDefinitionBuilder<MongoCacheObject<T>>();

            col.Indexes.CreateOne(keys.Ascending(x => x.Expiration), new CreateIndexOptions() { ExpireAfter = TimeSpan.Zero });

            var list = col.Find(x => filter.Compile().Invoke(x.Value)).ToList();

            return list.Select(x => x.Value).ToList();
        }

        private IMongoDatabase GetDatabase()
        {
            return _client.GetDatabase("cachemgr");
        }

        private IMongoCollection<T> GetCollection<T>(string name)
        {
            return GetDatabase().GetCollection<T>(name);
        }
    }

    public class MongoCacheObject<T>
    {
        public ObjectId Id { get; set; }
        public T Value { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
