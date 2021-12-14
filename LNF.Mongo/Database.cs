using MongoDB.Driver;

namespace LNF.Mongo
{
    public class Database
    {
        private readonly IMongoDatabase _db;

        internal Database(IMongoDatabase db)
        {
            _db = db;
        }

        public Collection<T> Collection<T>(string name) => new Collection<T>(_db.GetCollection<T>(name));
    }
}
