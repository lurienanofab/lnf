using LNF.Models.Data;

namespace LNF.Repository.Data
{
    public static class Extensions
    {
        public static ClientItem GetClientItem(this ClientInfo item)
        {
            if (item == null) return null;
            return item.Model<ClientItem>();
        }

        public static MenuItem GetMenuItem(this Menu item)
        {
            if (item == null) return null;
            return item.Model<MenuItem>();
        }
    }
}
