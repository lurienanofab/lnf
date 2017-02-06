using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ClientSettingMap : ClassMap<ClientSetting>
    {

        public ClientSettingMap()
        {
            Schema("sselScheduler.dbo");
            Table("ClientSetting");
            Id(x => x.ClientID).GeneratedBy.Assigned();
            Map(x => x.BuildingID);
            Map(x => x.LabID);
            Map(x => x.DefaultView);
            Map(x => x.BeginHour);
            Map(x => x.EndHour);
            Map(x => x.WorkDays);
            Map(x => x.EmailCreateReserv);
            Map(x => x.EmailModifyReserv);
            Map(x => x.EmailDeleteReserv);
            Map(x => x.EmailInvited);
            Map(x => x.AccountOrder);
        }
    }
}
