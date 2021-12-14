using LNF.Data;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class RoomRepository : ApiClient, IRoomRepository
    {
        internal RoomRepository(IRestClient rc) : base(rc) { }

        public IEnumerable<IRoom> GetActiveRooms(bool? parent = null)
        {
            if (parent.HasValue)
                return Get<List<RoomItem>>("webapi/data/room/active", QueryStrings(new { parent = parent.Value }));
            else
                return Get<List<RoomItem>>("webapi/data/room/active");
        }

        public IEnumerable<IPassbackRoom> GetPassbackRooms()
        {
            return Get<List<PassbackRoomItem>>("webapi/data/room/passback");
        }

        public IEnumerable<IRoom> GetRooms()
        {
            throw new NotImplementedException();
        }
    }
}
