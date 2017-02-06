using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using LNF.Repository.Email;

namespace LNF.Impl.Mappings.Email
{
    public class MassEmailMap : ClassMap<MassEmail>
    {
        public MassEmailMap()
        {
            Schema("Email.dbo");
            Id(x => x.MassEmailID);
            References(x => x.Client);
            Map(x => x.EmailId).Unique();
            Map(x => x.EmailFolder);
            Map(x => x.CreatedOn).CustomSqlType("datetime2").CustomType("datetime2");
            Map(x => x.ModifiedOn).CustomSqlType("datetime2").CustomType("datetime2");
            Map(x => x.RecipientGroup);
            Map(x => x.RecipientCriteria);
            Map(x => x.FromAddress);
            Map(x => x.CCAddress);
            Map(x => x.DisplayName);
            Map(x => x.Subject);
            Map(x => x.Body);
        }
    }
}
