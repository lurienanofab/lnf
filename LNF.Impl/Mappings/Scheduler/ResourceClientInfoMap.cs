using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ResourceClientInfoMap : ClassMap<ResourceClientInfo>
    {
        internal ResourceClientInfoMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ResourceClientInfo");
            ReadOnly();
            Id(x => x.ResourceClientID);
            Map(x => x.ResourceID);
            Map(x => x.ClientID);
            Map(x => x.UserName);
            Map(x => x.Privs);
            Map(x => x.AuthLevel);
            Map(x => x.Expiration);
            Map(x => x.EmailNotify);
            Map(x => x.PracticeResEmailNotify);
            Map(x => x.ResourceName);
            Map(x => x.AuthDuration);
            Map(x => x.ResourceIsActive);
            Map(x => x.DisplayName);
            Map(x => x.Email);
            Map(x => x.ClientActive);
        }
    }
}
