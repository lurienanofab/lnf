using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace LNF.Models.Scheduler
{
    [BsonIgnoreExtraElements]
    public class UserState
    {
        public int ClientID { get; set; }

        public ViewType View { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public DateTime AccessedAt { get; set; }

        public List<UserAction> Actions { get; set; }

        public static UserState Create(int clientId, ViewType view)
        {
            var result = new UserState()
            {
                ClientID = clientId,
                AccessedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                View = view,
                Actions = new List<UserAction>()
            };

            return result;
        }
    }
}
