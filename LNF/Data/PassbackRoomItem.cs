namespace LNF.Data
{
    public class PassbackRoomItem : IPassbackRoom
    {
        public int AreaID { get; set; }
        public int RoomID { get; set; }
        public string RoomDisplayName { get; set; }
        public string AreaName { get; set; }
    }
}
