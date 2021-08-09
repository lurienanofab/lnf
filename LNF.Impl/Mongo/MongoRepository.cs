using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Configuration;
using System.Linq.Expressions;

namespace LNF.Impl.Mongo
{
    public class MongoRepository
    {
        private readonly MongoClient _client;

        private readonly string _database;

        public MongoRepository(string database)
        {
            _database = database;
            var connstr = ConfigurationManager.AppSettings["MongoConnectionString"];
            _client = new MongoClient(connstr);
        }

        public void MapClass<T>(Expression<Func<T, Guid>> idExpression)
        {
            MongoUtility.RegisterClassMaps(idExpression);
        }

        public IEnumerable<T> Find<T>(string collection, Expression<Func<T, bool>> filter)
        {
            return GetCollection<T>(collection).Find(filter).ToEnumerable();
        }

        public IEnumerable<T> Find<T>(string collection, Filters<T> filters)
        {
            return GetCollection<T>(collection).Find(filters.Combine()).ToEnumerable();
        }

        public T Get<T>(string collection, Guid id)
        {
            var result = GetCollection<T>(collection).Find(Builders<T>.Filter.Eq("_id", id.ToString())).FirstOrDefault();
            return result;
        }

        public UpdateResult Update<T>(string collection, Expression<Func<T, bool>> filter, Updates<T> updates)
        {
            var result = GetCollection<T>(collection).UpdateOne(filter, updates.Combine());
            return new UpdateResult(result);
        }

        public void Update<T>(string collection, Filters<T> filters, Updates<T> updates)
        {
            GetCollection<T>(collection).UpdateOne(filters.Combine(), updates.Combine());
        }

        public void Delete<T>(string collection, Expression<Func<T, bool>> filter)
        {
            GetCollection<T>(collection).DeleteOne(filter);
        }

        public IEnumerable<T> Find<T>(string collection)
        {
            return GetCollection<T>(collection).Find(Builders<T>.Filter.Empty).ToEnumerable();
        }

        public void InsertOne<T>(string collection, T document)
        {
            GetCollection<T>(collection).InsertOne(document);
        }

        public Updates<T> Updates<T>()
        {
            return new Updates<T>();
        }

        public Filters<T> Filters<T>()
        {
            return new Filters<T>();
        }

        private IMongoDatabase GetDatabase()
        {
            return _client.GetDatabase(_database);
        }

        private IMongoCollection<T> GetCollection<T>(string collection)
        {
            return GetDatabase().GetCollection<T>(collection);
        }
    }
}
