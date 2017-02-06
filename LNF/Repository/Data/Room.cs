namespace LNF.Repository.Data
{
    public class Room : IDataItem
    {
        public virtual int RoomID { get; set; }
        public virtual int? ParentID { get; set; }
        public virtual string RoomName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual bool PassbackRoom { get; set; }
        public virtual bool Billable { get; set; }
        public virtual bool ApportionDailyFee { get; set; }
        public virtual bool ApportionEntryFee { get; set; }
        public virtual bool Active { get; set; }

        public virtual string GetDisplayNameOrDefault()
        {
            if (string.IsNullOrEmpty(DisplayName))
                return RoomName;
            else
                return DisplayName;
        }
    }
}
