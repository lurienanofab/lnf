namespace LNF.Models.Data
{
    public interface IRoom
    {
        int RoomID { get; set; }
        int? ParentID { get; set; }
        string RoomName { get; set; }
        string RoomDisplayName { get; set; }
        bool PassbackRoom { get; set; }
        bool Billable { get; set; }
        bool ApportionDailyFee { get; set; }
        bool ApportionEntryFee { get; set; }
        bool Active { get; set; }
    }
}