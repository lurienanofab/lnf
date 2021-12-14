using FluentNHibernate.Mapping;
using LNF.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ClientDemographicsMap : ClassMap<ClientDemographics>
    {
        internal ClientDemographicsMap()
        {
            Schema("sselData.dbo");
            Table("v_ClientDemographics");
            ReadOnly();
            Id(x => x.ClientID);
            Map(x => x.UserName).Not.Nullable();
            Map(x => x.LName);
            Map(x => x.MName);
            Map(x => x.FName);
            Map(x => x.Privs);
            Map(x => x.Active);
            Map(x => x.DemCitizenID);
            Map(x => x.DemCitizenValue);
            Map(x => x.DemGenderID);
            Map(x => x.DemGenderValue);
            Map(x => x.DemRaceID);
            Map(x => x.DemRaceValue);
            Map(x => x.DemEthnicID);
            Map(x => x.DemEthnicValue);
            Map(x => x.DemDisabilityID);
            Map(x => x.DemDisabilityValue);
        }
    }
}
