using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ServiceLogMap : ClassMap<ServiceLog>
    {
        public ServiceLogMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ServiceLogID);
            Map(x => x.ServiceName);
            Map(x => x.LogDateTime);
            Map(x => x.LogSubject);
            Map(x => x.LogLevel);
            Map(x => x.LogMessage);
            Map(x => x.MessageID);
            Map(x => x.Data).CustomType("StringClob").CustomSqlType("nvarchar(max)").Length(int.MaxValue);
        }
    }
};
