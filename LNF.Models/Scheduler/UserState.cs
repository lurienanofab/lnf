using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace LNF.Models.Scheduler
{
    public class UserState
    {
        public int ClientID { get; set; }

        [Obsolete("Should not be used but needed for backwards compatibility.")]
        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

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
                Date = DateTime.Now.Date,
                View = view,
                Actions = new List<UserAction>()
            };

            return result;
        }
    }
}
