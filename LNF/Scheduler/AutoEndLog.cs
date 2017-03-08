using LNF.Cache;
using LNF.Repository.Scheduler;
using MongoDB.Bson;
using System;

namespace LNF.Scheduler
{
    public static class AutoEndLog
    {
        public static void AddAutoEndEntry(Reservation rsv)
        {
            Add(new
            {
                Id = ObjectId.GenerateNewId(),
                ReservationID = rsv.ReservationID,
                ResourceID = rsv.Resource.ResourceID,
                ResourceName = rsv.Resource.ResourceName,
                ClientID = rsv.Client.ClientID,
                DisplayName = rsv.Client.DisplayName,
                Timestamp = DateTime.Now,
                Action = "autoend"
            });
        }

        public static void AddRepairEntry(Reservation rsv)
        {
            Add(new
            {
                Id = ObjectId.GenerateNewId(),
                ReservationID = rsv.ReservationID,
                ResourceID = rsv.Resource.ResourceID,
                ResourceName = rsv.Resource.ResourceName,
                ClientID = rsv.Client.ClientID,
                DisplayName = rsv.Client.DisplayName,
                Timestamp = DateTime.Now,
                Action = "repair"
            });
        }

        public static void AddUnstartedEntry(Reservation rsv)
        {
            Add(new
            {
                Id = ObjectId.GenerateNewId(),
                ReservationID = rsv.ReservationID,
                ResourceID = rsv.Resource.ResourceID,
                ResourceName = rsv.Resource.ResourceName,
                ClientID = rsv.Client.ClientID,
                DisplayName = rsv.Client.DisplayName,
                Timestamp = DateTime.Now,
                Action = "unstarted"
            });
        }

        private static void Add(object entry)
        {
            var doc = BsonDocument.Create(entry);
            var mongo = MongoRepository.Default.GetClient();
            var db = mongo.GetDatabase("logs");
            var col = db.GetCollection<BsonDocument>("autoend");
            col.InsertOne(doc);
        }
    }
}
