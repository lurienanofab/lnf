using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.CommonTools
{
    public static class ReadData
    {
        public static ReadRoomDataManager Room
        {
            get { return new ReadRoomDataManager(); }
        }

        public static ReadToolDataManager Tool
        {
            get { return new ReadToolDataManager(); }
        }

        public static ReadStoreDataManager Store(DateTime startDate, DateTime endDate, int clientId = 0, int itemId = 0)
        {
            return ReadStoreDataManager.Create(startDate, endDate, clientId, itemId);
        }

        public static ReadMiscDataManager Misc
        {
            get { return new ReadMiscDataManager(); }
        }
    }
}
