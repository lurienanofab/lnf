using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

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
            Map(x => x.Data);
        }
    }
};
