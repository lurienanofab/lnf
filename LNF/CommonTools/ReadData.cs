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

        public static ReadStoreDataManager Store
        {
            get { return new ReadStoreDataManager(); }
        }

        public static ReadMiscDataManager Misc
        {
            get { return new ReadMiscDataManager(); }
        }
    }
}
