using LNF.DataAccess;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class ClientSetting : IClientSetting, IDataItem
    {
        private ResourceTreeItemCollection _resourceTree;

        public ClientSetting() { }

        private ClientSetting(int clientId)
        {
            ClientID = clientId;
            BuildingID = DefaultBuildingID;
            LabID = DefaultLabID;
            DefaultView = DefaultDefaultView;
            BeginHour = DefaultBeginHour;
            EndHour = DefaultEndHour;
            WorkDays = DefaultWorkDays;
            EmailCreateReserv = DefaultEmailCreateReserv;
            EmailModifyReserv = DefaultEmailModifyReserv;
            EmailDeleteReserv = DefaultEmailDeleteReserv;
            EmailInvited = DefaultEmailInvited;
            AccountOrder = DefaultAccountOrder;
        }

        public static ClientSetting CreateWithDefaultValues(int clientId)
        {
            return new ClientSetting(clientId);
        }

        public virtual int ClientID { get; set; }
        public virtual int? BuildingID { get; set; }
        public virtual int? LabID { get; set; }
        public virtual int? DefaultView { get; set; }
        public virtual int? BeginHour { get; set; }
        public virtual int? EndHour { get; set; }
        public virtual string WorkDays { get; set; }
        public virtual bool? EmailCreateReserv { get; set; }
        public virtual bool? EmailModifyReserv { get; set; }
        public virtual bool? EmailDeleteReserv { get; set; }
        public virtual bool? EmailInvited { get; set; }
        public virtual string AccountOrder { get; set; }

        public virtual int DefaultBuildingID => 4;
        public virtual int DefaultLabID => 1;
        public virtual int DefaultDefaultView => (int)ViewType.DayView;
        public virtual int DefaultBeginHour => 8;
        public virtual int DefaultEndHour => 17;
        public virtual string DefaultWorkDays => "0,0,0,0,0,0,0";
        public virtual bool DefaultEmailCreateReserv => false;
        public virtual bool DefaultEmailModifyReserv => false;
        public virtual bool DefaultEmailDeleteReserv => false;
        public virtual bool DefaultEmailInvited => false;
        public virtual string DefaultAccountOrder => string.Empty;

        public virtual int GetBuildingID() => GetInt32OrDefault(BuildingID, DefaultBuildingID);
        public virtual int GetLabID() => GetInt32OrDefault(LabID, DefaultLabID);
        public virtual ViewType GetDefaultView() => (ViewType)GetInt32OrDefault(DefaultView, DefaultDefaultView);
        public virtual int GetBeginHour() => GetInt32OrDefault(BeginHour, DefaultBeginHour);
        public virtual int GetEndHour() => GetInt32OrDefault(EndHour, DefaultEndHour);
        public virtual string GetWorkDays() => WorkDays ?? DefaultWorkDays;
        public virtual bool GetEmailCreateReserv() => EmailCreateReserv.GetValueOrDefault(DefaultEmailCreateReserv);
        public virtual bool GetEmailModifyReserv() => EmailModifyReserv.GetValueOrDefault(DefaultEmailModifyReserv);
        public virtual bool GetEmailDeleteReserv() => EmailDeleteReserv.GetValueOrDefault(DefaultEmailDeleteReserv);
        public virtual bool GetEmailInvited() => EmailInvited.GetValueOrDefault(DefaultEmailInvited);
        public virtual string GetAccountOrder() => AccountOrder ?? DefaultAccountOrder;
        public virtual bool IsValid() => ClientID != 0;

        public virtual ResourceTreeItemCollection GetResourceTree(IProvider provider)
        {
            if (_resourceTree == null)
            {
                var resources = provider.Scheduler.Resource.GetResourceTree(ClientID);
                _resourceTree = new ResourceTreeItemCollection(resources);
            }

            return _resourceTree;
        }

        public virtual IBuilding GetBuildingOrDeafult(IProvider provider)
        {
            int buildingId = BuildingID.GetValueOrDefault(DefaultBuildingID);

            if (buildingId == -1)
                return GetResourceTree(provider).GetBuilding(DefaultBuildingID);
            else
                return GetResourceTree(provider).GetBuilding(buildingId);
        }

        public virtual ILab GetLabOrDefault(IProvider provider)
        {
            int labId = LabID.GetValueOrDefault(DefaultLabID);

            if (labId == -1)
                return GetResourceTree(provider).GetLab(DefaultLabID);
            else
                return GetResourceTree(provider).GetLab(labId);
        }

        private int GetInt32OrDefault(int? v, int defval)
        {
            if (!v.HasValue || v.Value == -1)
                return defval;
            else
                return v.Value;
        }
    }
}
