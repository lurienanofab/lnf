using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using LNF.Models.Ordering;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using LNF.Repository.Scheduler;
using Omu.ValueInjecter;
using System;

namespace LNF.Impl.ModelFactory
{
    public static class ModelBuilder
    {
        public static class Data
        {
            public static IAccountManager AccountManager => DA.Use<IAccountManager>();

            public static ClientAccountItem CreateClientAccountModel(ClientAccount source)
            {
                var result = new ClientAccountItem();

                result.InjectFrom(source);
                result.IsDefault = source.IsDefault.GetValueOrDefault();
                result.ClientAccountActive = source.Active;
                result.AccountID = source.Account.AccountID;
                result.AccountName = source.Account.Name;
                result.Number = source.Account.Number;
                result.ShortCode = source.Account.ShortCode;
                result.BillAddressID = source.Account.BillAddressID;
                result.ShipAddressID = source.Account.ShipAddressID;
                result.InvoiceNumber = source.Account.InvoiceNumber;
                result.InvoiceLine1 = source.Account.InvoiceLine1;
                result.InvoiceLine2 = source.Account.InvoiceLine2;
                result.PoEndDate = source.Account.PoEndDate;
                result.PoInitialFunds = source.Account.PoInitialFunds;
                result.PoRemainingFunds = source.Account.PoRemainingFunds;
                result.AccountActive = source.Account.Active;
                result.FundingSourceID = source.Account.FundingSourceID;
                result.FundingSourceName = AccountManager.FundingSourceName(source.Account);
                result.TechnicalFieldID = source.Account.TechnicalFieldID;
                result.TechnicalFieldName = AccountManager.TechnicalFieldName(source.Account);
                result.SpecialTopicID = source.Account.SpecialTopicID;
                result.SpecialTopicName = AccountManager.SpecialTopicName(source.Account);
                result.AccountTypeID = source.Account.AccountType.AccountTypeID;
                result.AccountTypeName = source.Account.AccountType.AccountTypeName;
                result.ClientOrgID = source.ClientOrg.ClientOrgID;
                result.Phone = source.ClientOrg.Phone;
                result.Email = source.ClientOrg.Email;
                result.IsManager = source.ClientOrg.IsManager;
                result.ClientOrgActive = source.ClientOrg.Active;
                result.OrgID = source.ClientOrg.Org.OrgID;
                result.OrgName = source.ClientOrg.Org.OrgName;
                result.OrgActive = source.ClientOrg.Org.Active;
                result.ClientID = source.ClientOrg.Client.ClientID;
                result.LName = source.ClientOrg.Client.LName;
                result.MName = source.ClientOrg.Client.MName;
                result.FName = source.ClientOrg.Client.FName;
                result.DisplayName = source.ClientOrg.Client.DisplayName;
                result.Privs = source.ClientOrg.Client.Privs;
                result.ClientActive = source.ClientOrg.Client.Active;

                return result;
            }

            public static GlobalCostItem CreateGlobalCostModel(GlobalCost source)
            {
                var result = new GlobalCostItem();

                result.InjectFrom(source);
                result.AdminID = source.Admin.ClientID;
                result.LabAccountID = source.LabAccount.AccountID;
                result.LabCreditAccountID = source.LabCreditAccount.AccountID;
                result.SubsidyCreditAccountID = source.SubsidyCreditAccount.AccountID;

                return result;
            }

            public static RoomItem CreateRoomModel(Room source)
            {
                var result = new RoomItem();

                result.InjectFrom(source);
                result.RoomDisplayName = source.DisplayName;

                return result;
            }
        }

        public static class Scheduler
        {
            public static IClientOrgManager ClientOrgManager => DA.Use<IClientOrgManager>();

            public static ReservationItem CreateReservationModel(Reservation source)
            {
                var result = new ReservationItem();

                result.InjectFrom(source);
                result.ResourceID = source.Resource.ResourceID;
                result.ResourceName = source.Resource.ResourceName;
                result.ClientID = source.Client.ClientID;
                result.LName = source.Client.LName;
                result.FName = source.Client.FName;
                result.AccountID = source.Account.AccountID;
                result.AccountName = source.Account.Name;
                result.ShortCode = source.Account.ShortCode;
                result.ActivityID = source.Activity.ActivityID;
                result.ActivityName = source.Activity.ActivityName;

                return result;
            }

            public static BuildingModel CreateBuildingModel(Building source)
            {
                var result = new BuildingModel();

                result.InjectFrom(source);
                result.BuildingDescription = source.Description;
                result.BuildingIsActive = source.IsActive;

                return result;
            }

            public static LabModel CreateLabModel(Lab source)
            {
                int roomId = 0;
                string roomName = string.Empty;
                string roomDisplayName = string.Empty;

                if (source.Room != null)
                {
                    roomId = source.Room.RoomID;
                    roomName = source.Room.RoomName;
                    roomDisplayName = source.Room.DisplayName;
                }

                var result = new LabModel();

                result.InjectFrom(source);
                result.BuildingID = source.Building.BuildingID;
                result.BuildingName = source.Building.BuildingName;
                result.BuildingIsActive = source.Building.IsActive;
                result.RoomID = roomId;
                result.RoomName = roomName;
                result.RoomDisplayName = roomDisplayName;
                result.LabDescription = source.Description;
                result.LabIsActive = source.IsActive;

                return result;
            }

            public static ProcessTechModel CreateProcessTechModel(ProcessTech source)
            {
                var result = new ProcessTechModel();

                result.InjectFrom(source);
                result.GroupID = source.Group.ProcessTechGroupID;
                result.GroupName = source.Group.GroupName;
                result.LabID = source.Lab.LabID;
                result.LabName = source.Lab.LabName;
                result.LabDisplayName = source.Lab.DisplayName;
                result.LabIsActive = source.Lab.IsActive;
                result.BuildingID = source.Lab.Building.BuildingID;
                result.BuildingName = source.Lab.Building.BuildingName;
                result.BuildingIsActive = source.Lab.Building.IsActive;
                result.ProcessTechDescription = source.Description;
                result.ProcessTechIsActive = source.IsActive;

                return result;
            }

            public static ResourceModel CreateResourceModel(Resource source)
            {
                var result = new ResourceModel();

                result.InjectFrom(source);
                result.BuildingID = source.ProcessTech.Lab.Building.BuildingID;
                result.BuildingName = source.ProcessTech.Lab.Building.BuildingName;
                result.LabID = source.ProcessTech.Lab.LabID;
                result.LabName = source.ProcessTech.Lab.LabName;
                result.LabDisplayName = source.ProcessTech.Lab.DisplayName;
                result.ProcessTechID = source.ProcessTech.ProcessTechID;
                result.ProcessTechName = source.ProcessTech.ProcessTechName;
                result.Granularity = TimeSpan.FromMinutes(source.Granularity);
                result.ReservFence = TimeSpan.FromMinutes(source.ReservFence);
                result.MinReservTime = TimeSpan.FromMinutes(source.MinReservTime);
                result.MaxReservTime = TimeSpan.FromMinutes(source.MaxReservTime);
                result.MaxAlloc = TimeSpan.FromMinutes(source.MaxAlloc);
                result.Offset = TimeSpan.FromHours(source.Offset);
                result.GracePeriod = TimeSpan.FromMinutes(source.GracePeriod);
                result.AutoEnd = TimeSpan.FromMinutes(source.AutoEnd);
                result.MinCancelTime = TimeSpan.FromMinutes(source.MinCancelTime);
                result.UnloadTime = TimeSpan.FromMinutes(source.UnloadTime.GetValueOrDefault(0));
                result.ResourceDescription = source.Description;
                result.ResourceIsActive = source.IsActive;

                return result;
            }

            public static ResourceModel CreateResourceModel(ResourceInfo source)
            {
                var result = new ResourceModel();

                result.InjectFrom(source);
                result.Granularity = TimeSpan.FromMinutes(source.Granularity);
                result.ReservFence = TimeSpan.FromMinutes(source.ReservFence);
                result.MinReservTime = TimeSpan.FromMinutes(source.MinReservTime);
                result.MaxReservTime = TimeSpan.FromMinutes(source.MaxReservTime);
                result.MaxAlloc = TimeSpan.FromMinutes(source.MaxAlloc);
                result.Offset = TimeSpan.FromHours(source.Offset);
                result.GracePeriod = TimeSpan.FromMinutes(source.GracePeriod);
                result.AutoEnd = TimeSpan.FromMinutes(source.AutoEnd);
                result.MinCancelTime = TimeSpan.FromMinutes(source.MinCancelTime);
                result.UnloadTime = TimeSpan.FromMinutes(source.UnloadTime.GetValueOrDefault(0));
                result.State = (ResourceState)source.State;
                result.ResourceIsActive = source.IsActive;

                return result;
            }

            public static ProcessInfoModel CreateProcessInfoModel(ProcessInfo source)
            {
                var result = new ProcessInfoModel();

                result.InjectFrom(source);
                result.ResourceID = source.Resource.ResourceID;
                result.ResourceName = source.Resource.ResourceName;

                return result;
            }

            public static ProcessInfoLineModel CreateProcessInfoLineModel(ProcessInfoLine source)
            {
                var result = new ProcessInfoLineModel();

                result.InjectFrom(source);
                result.ProcessInfoLineParamID = source.ProcessInfoLineParam.ProcessInfoLineParamID;
                result.ResourceID = source.ProcessInfoLineParam.Resource.ResourceID;
                result.ResourceName = source.ProcessInfoLineParam.Resource.ResourceName;
                result.ParameterName = source.ProcessInfoLineParam.ParameterName;
                result.ParameterType = source.ProcessInfoLineParam.ParameterType;

                return result;
            }

            public static ResourceClientModel CreateResourceClientModel(ResourceClient source)
            {
                string userName = string.Empty;
                string displayName = "Everyone";
                string email = string.Empty;
                bool clientActive = true;
                ClientPrivilege privs = 0;

                if (source.ClientID != -1)
                {
                    var c = CacheManager.Current.GetClient(source.ClientID);

                    userName = c.UserName;
                    displayName = c.DisplayName;

                    var primary = ClientOrgManager.GetPrimary(source.ClientID);

                    if (primary != null)
                        email = primary.Email;
                }

                var result = new ResourceClientModel();

                result.InjectFrom(source);
                result.ResourceID = source.Resource.ResourceID;
                result.ResourceName = source.Resource.ResourceName;
                result.UserName = userName;
                result.Privs = privs;
                result.DisplayName = displayName;
                result.Email = email;
                result.ClientActive = clientActive;

                return result;
            }

            public static ResourceCostModel CreateResourceCostModel(Cost source)
            {
                var result = new ResourceCostModel();

                result.InjectFrom(source);
                result.ResourceID = source.RecordID;
                result.ChargeTypeID = source.ChargeTypeID;
                result.ChargeTypeName = source.GetChargeType().ChargeTypeName;

                return result;
            }
        }

        public static class Ordering
        {
            public static ApproverItem CreateApproverModel(Approver source)
            {
                var result = new ApproverItem();

                result.InjectFrom(source);

                var approver = CacheManager.Current.GetClient(source.ApproverID);
                var client = CacheManager.Current.GetClient(source.ClientID);

                result.ApproverDisplayName = approver != null ? approver.DisplayName : string.Empty;
                result.DisplayName = client != null ? client.DisplayName : string.Empty;

                return result;
            }
        }
    }
}
