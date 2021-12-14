using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LNF.Mongo
{
    public class Repository
    {
        private readonly IMongoClient _client;

        public Repository(string connstr)
        {
            _client = new MongoClient(connstr);
        }

        public Database Database(string name)
        {
            return new Database(_client.GetDatabase(name));
        }
    }
}
