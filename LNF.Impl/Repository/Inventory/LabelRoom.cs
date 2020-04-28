using LNF.DataAccess;

namespace LNF.Impl.Repository.Inventory
{
    public class LabelRoom : IDataItem
    {
        public virtual int LabelRoomID { get; set; }
        public virtual string Slug { get; set; }
        public virtual string RoomName { get; set; }
    }
}
