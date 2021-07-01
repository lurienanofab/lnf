using LNF.Data;
using LNF.Impl.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Data
{
    public static class Extensions
    {
        public static IEnumerable<IStaffDirectory> CreateModels(this IEnumerable<StaffDirectory> source, IEnumerable<IClient> staff)
        {
            return source.ToList().Select(x => x.CreateModel(staff)).ToList();
        }

        public static IStaffDirectory CreateModel(this StaffDirectory source, IEnumerable<IClient> staff)
        {
            var c = staff.First(x => x.ClientID == source.Client.ClientID);

            return new StaffDirectoryItem
            {
                StaffDirectoryID = source.StaffDirectoryID,
                ClientID = source.Client.ClientID,
                LName = c.LName,
                FName = c.FName,
                ContactEmail = c.Email,
                HoursXML = source.HoursXML,
                ContactPhone = source.ContactPhone,
                Office = source.Office,
                Deleted = source.Deleted,
                LastUpdate = source.LastUpdate
            };
        }
    }
}
