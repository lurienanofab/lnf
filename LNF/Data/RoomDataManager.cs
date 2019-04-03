using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Linq;

namespace LNF.Data
{
    public class RoomDataManager : ManagerBase, IRoomDataManager
    {
        public RoomDataManager(IProvider provider) : base(provider) { }

        public string GetEmail(RoomData item)
        {
            var ca = Session
                .Query<ClientAccountInfo>()
                .FirstOrDefault(x => x.ClientID == item.ClientID && x.AccountID == item.AccountID);

            if (ca == null) return string.Empty;

            return ca.Email;
        }

        public RoomData Create(RoomDataClean rdc)
        {
            throw new NotImplementedException();
        }
    }
}
