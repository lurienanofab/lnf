using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ClientMap : ClassMap<Client>
    {
        public ClientMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ClientID);
            Map(x => x.FName);
            Map(x => x.MName);
            Map(x => x.LName);
            Map(x => x.UserName).Not.Nullable();
            Map(x => x.DemCitizenID);
            Map(x => x.DemGenderID);
            Map(x => x.DemRaceID);
            Map(x => x.DemEthnicID);
            Map(x => x.DemDisabilityID);
            Map(x => x.Privs);
            Map(x => x.Communities);
            Map(x => x.TechnicalFieldID, "TechnicalInterestID");
            Map(x => x.Active);
            Map(x => x.IsChecked, "isChecked");
            Map(x => x.IsSafetyTest, "isSafetyTest");
        }
    }
}
