using MongoDB.Driver;
using System.Configuration;

namespace LNF.Cache
{
    public class MongoRepository
    {
        public static MongoRepository Default { get; }

        static MongoRepository()
        {
            Default = new MongoRepository();
        }

        private readonly MongoClient _client;

        private MongoRepository()
        {
            _client = new MongoClient(ConfigurationManager.AppSettings["MongoConnectionString"]);
        }

        public IMongoClient GetClient()
        {
            return _client;
        }
    }
}
