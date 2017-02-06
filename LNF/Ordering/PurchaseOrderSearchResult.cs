using LNF.Cache;
using LNF.Models.Ordering;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Ordering
{
    public class PurchaseOrderSearchResult
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public int ClientID { get; set; }

        public DateTime CreatedAt { get; set; }

        public IList<PurchaseOrderSearchModel> Items { get; set; }

        private static IMongoCollection<PurchaseOrderSearchResult> GetCollection()
        {
            return MongoRepository.Default.GetClient().GetDatabase("ordering")
                .GetCollection<PurchaseOrderSearchResult>("iofsearch")
                .Expire(TimeSpan.FromMinutes(15), x => x.CreatedAt)
                .Unique(x => x.ClientID);
        }

        public static PurchaseOrderSearchResult Get(Expression<Func<PurchaseOrderSearchResult, bool>> filter)
        {
            return GetCollection().Find(filter).FirstOrDefault();
        }

        public static PurchaseOrderSearchResult Set(int clientId, IEnumerable<PurchaseOrderSearchModel> items)
        {
            PurchaseOrderSearchResult result = new PurchaseOrderSearchResult()
            {
                ClientID = clientId,
                CreatedAt = DateTime.Now,
                Items = items.ToList()
            };

            GetCollection()
                .ReplaceOne(x => x.ClientID == clientId, result, new UpdateOptions() { IsUpsert = true });

            return result;
        }

        public static void Delete(int clientId)
        {
            GetCollection().DeleteOne(x => x.ClientID == clientId);
        }
    }
}
