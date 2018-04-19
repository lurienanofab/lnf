using LNF.Data;
using LNF.Repository;

namespace LNF.CommonTools
{
    public static class ReadData
    {
        public static IReadRoomDataManager Room
        {
            get { return DA.Use<IReadRoomDataManager>(); }
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
