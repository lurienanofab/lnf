using LNF.Data;

namespace LNF.Scheduler
{
    public class ExpiringCardConverter : IDataFeedResultItemConverter<ExpiringCard>
    {
        public string Key => "default";

        public ExpiringCard Convert(DataFeedResultItem item)
        {
            return new ExpiringCard
            {
                BadgeExpireDate = System.Convert.ToDateTime(item["BadgeExpireDate"]),
                CardExpireDate = System.Convert.ToDateTime(item["CardExpireDate"]),
                CardNumber = item["CardNumber"],
                CardStatus = item["CardStatus"],
                Email = item["Email"],
                FName = item["FName"],
                LabUser = System.Convert.ToBoolean(item["LabUser"]),
                LName = item["LName"],
                PhysicalAccess = System.Convert.ToBoolean(item["PhysicalAccess"]),
                Staff = System.Convert.ToBoolean(item["Staff"]),
                ClientID = System.Convert.ToInt32(item["ClientID"])
            };
        }
    }
}
