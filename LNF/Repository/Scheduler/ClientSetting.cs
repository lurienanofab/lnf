using LNF.Cache;
using LNF.Models.Scheduler;
using LNF.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Scheduler
{
    public class ClientSetting : IDataItem
    {
        private ResourceTreeItemCollection _resourceTree;

        public static readonly int DefaultBuildingID = 4;
        public static readonly int DefaultLabID = 1;
        public static readonly int DefaultDefaultView = (int)ViewType.DayView;
        public static readonly int DefaultBeginHour = 8;
        public static readonly int DefaultEndHour = 17;
        public static readonly string DefaultWorkDays = "0,0,0,0,0,0,0";
        public static readonly bool DefaultEmailCreateReserv = false;
        public static readonly bool DefaultEmailModifyReserv = false;
        public static readonly bool DefaultEmailDeleteReserv = false;
        public static readonly bool DefaultEmailInvited = false;
        public static readonly string DefaultAccountOrder = string.Empty;

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

        public virtual ResourceTreeItemCollection GetResourceTree()
        {
            if (_resourceTree == null)
            {
                var items = ServiceProvider.Current.Scheduler.Resource.GetResourceTree(ClientID);
                _resourceTree = new ResourceTreeItemCollection(items);
            }

            return _resourceTree;
        }

        public virtual bool IsValid()
        {
            return ClientID != 0;
        }

        public virtual ViewType GetDefaultViewOrDefault()
        {
            return (ViewType)DefaultView.GetValueOrDefault(DefaultDefaultView);
        }

        public virtual int GetBeginHourOrDefault()
        {
            return BeginHour.GetValueOrDefault(DefaultBeginHour);
        }

        public virtual int GetEndHourOrDefault()
        {
            return EndHour.GetValueOrDefault(DefaultEndHour);
        }

        public virtual string GetWorkDaysOrDefault()
        {
            return string.IsNullOrEmpty(WorkDays) ? DefaultWorkDays : WorkDays;
        }

        public virtual bool GetEmailCreateReservOrDefault()
        {
            return EmailCreateReserv.GetValueOrDefault(DefaultEmailCreateReserv);
        }

        public virtual bool GetEmailModifyReservOrDefault()
        {
            return EmailModifyReserv.GetValueOrDefault(DefaultEmailModifyReserv);
        }

        public virtual bool GetEmailDeleteReservOrDefault()
        {
            return EmailDeleteReserv.GetValueOrDefault(DefaultEmailDeleteReserv);
        }

        public virtual bool GetEmailInvitedOrDefault()
        {
            return EmailInvited.GetValueOrDefault(DefaultEmailInvited);
        }

        public virtual string GetAccountOrderOrDefault()
        {
            return string.IsNullOrEmpty(AccountOrder) ? DefaultAccountOrder : AccountOrder;
        }

        public virtual BuildingItem GetBuildingOrDeafult()
        {
            int buildingId = BuildingID.GetValueOrDefault(DefaultBuildingID);

            if (buildingId == -1)
                return GetResourceTree().GetBuilding(DefaultBuildingID);
            else
                return GetResourceTree().GetBuilding(buildingId);
        }

        public virtual LabItem GetLabOrDefault()
        {
            int labId = LabID.GetValueOrDefault(DefaultLabID);

            if (labId == -1)
                return GetResourceTree().GetLab(DefaultLabID);
            else
                return GetResourceTree().GetLab(labId);
        }

        public static ClientSetting GetClientSettingOrDefault(int clientId)
        {
            var cs = DA.Current.Single<ClientSetting>(clientId);

            if (cs == null)
            {
                cs = CreateWithDefaultValues(clientId);
                DA.Current.Insert(cs); //creates a new record in the db
            }

            return cs;
        }

        public static ClientSetting CreateWithDefaultValues(int clientId)
        {
            return new ClientSetting()
            {
                ClientID = clientId,
                BuildingID = DefaultBuildingID,
                LabID = DefaultLabID,
                DefaultView = DefaultDefaultView,
                BeginHour = DefaultBeginHour,
                EndHour = DefaultEndHour,
                WorkDays = DefaultWorkDays,
                EmailCreateReserv = DefaultEmailCreateReserv,
                EmailModifyReserv = DefaultEmailModifyReserv,
                EmailDeleteReserv = DefaultEmailDeleteReserv,
                EmailInvited = DefaultEmailInvited,
                AccountOrder = DefaultAccountOrder
            };
        }
    }
}
