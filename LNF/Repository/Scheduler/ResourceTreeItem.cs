using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Scheduler
{
    public class ResourceTreeItem : IDataItem
    {
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual string ResourceDescription { get; set; }
        public virtual bool ResourceIsActive { get; set; }
        public virtual int ProcessTechID { get; set; }
        public virtual string ProcessTechName { get; set; }
        public virtual string ProcessTechDescription { get; set; }
        public virtual bool ProcessTechIsActive { get; set; }
        public virtual int ProcessTechGroupID { get; set; }
        public virtual string ProcessTechGroupName { get; set; }
        public virtual int LabID { get; set; }
        public virtual string LabName { get; set; }
        public virtual string LabDisplayName { get; set; }
        public virtual string LabDescription { get; set; }
        public virtual bool LabIsActive { get; set; }
        public virtual int RoomID { get; set; }
        public virtual string RoomName { get; set; }
        public virtual int BuildingID { get; set; }
        public virtual string BuildingName { get; set; }
        public virtual string BuildingDescription { get; set; }
        public virtual bool BuildingIsActive { get; set; }
        public virtual bool IsSchedulable { get; set; }
        public virtual string HelpdeskEmail { get; set; }
        public virtual string WikiPageUrl { get; set; }
        public virtual ResourceState State { get; set; }
        public virtual string StateNotes { get; set; }
        public virtual int AuthDuration { get; set; }
        public virtual bool AuthState { get; set; }
        public virtual int ReservFence { get; set; }
        public virtual int MaxAlloc { get; set; }
        public virtual int MinCancelTime { get; set; }
        public virtual int AutoEnd { get; set; }
        public virtual int UnloadTime { get; set; }
        public virtual int Granularity { get; set; }
        public virtual int Offset { get; set; }
        public virtual bool IsReady { get; set; }
        public virtual int MinReservTime { get; set; }
        public virtual int MaxReservTime { get; set; }
        public virtual int GracePeriod { get; set; }
        public virtual int CurrentReservationID { get; set; }
        public virtual int CurrentClientID { get; set; }
        public virtual int CurrentActivityID { get; set; }
        public virtual string CurrentFirstName { get; set; }
        public virtual string CurrentLastName { get; set; }
        public virtual string CurrentActivityName { get; set; }
        public virtual bool CurrentActivityEditable { get; set; }
        public virtual DateTime? CurrentBeginDateTime { get; set; }
        public virtual DateTime? CurrentEndDateTime { get; set; }
        public virtual string CurrentNotes { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual int Privs { get; set; }
        public virtual int Communities { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual bool ClientActive { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual int MaxChargeTypeID { get; set; }
        public virtual int ResourceClientID { get; set; }
        public virtual int AuthLevel { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual int? EmailNotify { get; set; }
        public virtual int? PracticeResEmailNotify { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as ResourceTreeItem;
            if (item == null) return false;
            return item.ResourceID == ResourceID && item.ClientID == ClientID;
        }

        public override int GetHashCode()
        {
            return new { ResourceID, ClientID }.GetHashCode();
        }

        public override string ToString()
        {
            return ResourceModel.GetDisplayName(ResourceName, ResourceID);
        }

        public virtual IEnumerable<ResourceClientInfo> GetToolEngineers()
        {
            return DA.Current.Query<ResourceClientInfo>()
                .Where(x => x.ResourceID == ResourceID && (x.AuthLevel & ClientAuthLevel.ToolEngineer) > 0)
                .OrderBy(x => x.DisplayName).ToList();
        }

        public virtual ResourceCost GetCost(ClientItem client)
        {
            var maxChargeTypeId = client.MaxChargeTypeID;
            var tables = new[] { "ToolCost", "ToolOvertimeCost" };

            var costs = DA.Current.Query<Cost>().Where(x => x.RecordID == ResourceID && tables.Contains(x.TableNameOrDescription)).ToList();
            var result = new ResourceCost(costs, ResourceID, maxChargeTypeId);

            return result;
        }
    }
}
