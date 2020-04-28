using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ServiceTaskMap : ClassMap<ServiceTask>
    {
        internal ServiceTaskMap()
        {
            Schema("sselData.dbo");
            Table("ServiceTask");
            Id(x => x.ServiceTaskID);
            Map(x => x.MessageID);
            Map(x => x.TaskName);
            Map(x => x.StartTime);
            Map(x => x.EndTime);
            Map(x => x.Options);
            Map(x => x.ErrorMessage);
        }
    }
}
