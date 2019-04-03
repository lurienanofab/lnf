namespace LNF.CommonTools
{
    public static class ReadData
    {
        public static IReadRoomDataManager Room => ServiceProvider.Current.ReadRoomDataManager;

        public static IReadToolDataManager Tool => ServiceProvider.Current.ReadToolDataManager;

        public static IReadStoreDataManager Store => ServiceProvider.Current.ReadStoreDataManager;

        public static IReadMiscDataManager Misc => ServiceProvider.Current.ReadMiscDataManager;
    }
}
