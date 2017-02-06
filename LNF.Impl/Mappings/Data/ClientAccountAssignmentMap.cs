using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class ClientAccountAssignmentMap : ClassMap<ClientAccountAssignment>
    {
        public ClientAccountAssignmentMap()
        {
            Schema("sselData.dbo");
            Table("v_ClientAccountAssignment");
            ReadOnly();
            CompositeId()
                .KeyProperty(x => x.ClientManagerID)
                .KeyProperty(x => x.ManagerClientAccountID);
            Map(x=>x.ClientOrgID);
            Map(x=>x.ManagerOrgID);
            Map(x=>x.ClientManagerActive);
            Map(x=>x.ManagerClientAccountActive);
            Map(x=>x.Manager);
            Map(x=>x.AccountID);
            Map(x=>x.AccountName);
            Map(x=>x.AccountNumber);
            Map(x=>x.ShortCode);
            Map(x=>x.AccountActive);
            Map(x=>x.ManagerEmail);
            Map(x=>x.ManagerClientOrgActive);
            Map(x=>x.ManagerLastName);
            Map(x=>x.ManagerFirstName);
            Map(x=>x.ManagerPrivs);
            Map(x=>x.ManagerClientActive);
            Map(x=>x.EmployeeClientAccountID);
            Map(x=>x.EmployeeClientAccountActive);
            Map(x=>x.EmployeeEmail);
            Map(x=>x.EmployeeClientOrgActive);
            Map(x=>x.EmployeeLastName);
            Map(x=>x.EmployeeFirstName);
            Map(x=>x.EmployeePrivs);
            Map(x=>x.EmployeeClientActive);
            Map(x=>x.HasDryBox);
        }
    }
}
