using LNF.Repository;

namespace LNF.CommonTools
{
    public static class ReadData
    {
        public static IReadRoomDataManager Room => ServiceProvider.Current.Use<IReadRoomDataManager>();

        public static IReadToolDataManager Tool => ServiceProvider.Current.Use<IReadToolDataManager>();

        public static IReadStoreDataManager Store => ServiceProvider.Current.Use<IReadStoreDataManager>();

        public static IReadMiscDataManager Misc => ServiceProvider.Current.Use<IReadMiscDataManager>();
    }
}
