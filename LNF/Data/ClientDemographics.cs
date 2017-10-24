using LNF.Repository;
using LNF.Repository.Data;
using System;

namespace LNF.Data
{
    public class ClientDemographics
    {
        public DemCitizen Citizen { get; }
        public DemEthnic Ethnic { get; }
        public DemRace Race { get; }
        public DemGender Gender { get; }
        public DemDisability Disability { get; }

        public ClientDemographics(DemCitizen c, DemEthnic e, DemRace r, DemGender g, DemDisability d)
        {
            Citizen = c ?? throw new ArgumentNullException("c");
            Ethnic = e ?? throw new ArgumentNullException("e");
            Race = r ?? throw new ArgumentNullException("r");
            Gender = g ?? throw new ArgumentNullException("g");
            Disability = d ?? throw new ArgumentNullException("d");
        }

        public static ClientDemographics Create(Client c)
        {
            if (c == null)
                throw new ArgumentNullException("c");

            ClientDemographics result = new ClientDemographics(
                c: DA.Current.Single<DemCitizen>(c.DemCitizenID),
                e: DA.Current.Single<DemEthnic>(c.DemEthnicID),
                r: DA.Current.Single<DemRace>(c.DemRaceID),
                g: DA.Current.Single<DemGender>(c.DemGenderID),
                d: DA.Current.Single<DemDisability>(c.DemDisabilityID));

            return result;
        }

        public void Update(Client client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            if (Citizen != null)
                client.DemCitizenID = Citizen.DemCitizenID;

            if (Ethnic != null)
                client.DemEthnicID = Ethnic.DemEthnicID;

            if (Race != null)
                client.DemRaceID = Race.DemRaceID;

            if (Gender != null)
                client.DemGenderID = Gender.DemGenderID;

            if (Disability != null)
                client.DemDisabilityID = Disability.DemDisabilityID;
        }
    }
}
