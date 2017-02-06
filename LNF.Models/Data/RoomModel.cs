namespace LNF.Models.Data
{
    public class RoomModel
    {
        public int RoomID { get; set; }
        public int? ParentID { get; set; }
        public string RoomName { get; set; }
        public string DisplayName { get; set; }
        public bool PassbackRoom { get; set; }
        public bool Billable { get; set; }
        public bool ApportionDailyFee { get; set; }
        public bool ApportionEntryFee { get; set; }
        public bool Active { get; set; }

        public string GetDisplayNameOrDefault()
        {
            if (string.IsNullOrEmpty(DisplayName))
                return RoomName;
            else
                return DisplayName;
        }
    }
}
