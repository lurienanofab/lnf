namespace LNF.Models.Data
{
    public interface IPassbackRoom
    {
        int AreaID { get; set; }
        int RoomID { get; set; }
        string RoomDisplayName { get; set; }
        string AreaName { get; set; }
    }
}
