using LNF.Billing;

namespace LNF.CommonTools
{
    public static class ReadData
    {
        public static IRoomDataRepository Room => ServiceProvider.Current.Billing.RoomData;

        public static IToolDataRepository Tool => ServiceProvider.Current.Billing.ToolData;

        public static IStoreDataRepository Store => ServiceProvider.Current.Billing.StoreData;

        public static IMiscDataRepository Misc => ServiceProvider.Current.Billing.MiscData;
    }
}
