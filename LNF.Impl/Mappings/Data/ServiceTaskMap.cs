using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ServiceTaskMap : ClassMap<ServiceTask>
    {
        public ServiceTaskMap()
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
