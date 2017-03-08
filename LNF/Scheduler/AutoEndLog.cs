using LNF.Cache;
using LNF.Repository.Scheduler;
using MongoDB.Bson;
using System;

namespace LNF.Scheduler
{
    public static class AutoEndLog
    {
        public static void AddEntry(Reservation rsv, string action)
        {
            var document = new BsonDocument();

            document
                .Add("ReservationID", new BsonInt32(rsv.ReservationID))
                .Add("ResourceID", new BsonInt32(rsv.Resource.ResourceID))
                .Add("ResourceName", new BsonString(rsv.Resource.ResourceName))
                .Add("ClientID", new BsonInt32(rsv.Client.ClientID))
                .Add("DisplayName", new BsonString(rsv.Client.DisplayName))
                .Add("Timestamp", new BsonDateTime(DateTime.Now))
                .Add("Action", new BsonString(action));

            var mongo = MongoRepository.Default.GetClient();
            var db = mongo.GetDatabase("logs");
            var col = db.GetCollection<BsonDocument>("autoend");
            col.InsertOne(document);
        }
    }
}
