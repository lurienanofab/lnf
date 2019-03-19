using LNF.Models.Scheduler;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Scheduler
{
    public static class Extensions
    {
        public static ResourceClientItem CreateResourceClientItem(this ResourceClientInfo item)
        {
            if (item == null) return null;
            var list = new List<ResourceClientInfo> { item };
            return CreateResourceClientItems(list.AsQueryable()).FirstOrDefault();
        }

        public static IEnumerable<ResourceClientItem> CreateResourceClientItems(this IQueryable<ResourceClientInfo> query)
        {
            if (query == null) return null;

            return query.Select(x => new ResourceClientItem
            {
                ResourceClientID = x.ResourceClientID,
                ResourceID = x.ResourceID,
                ResourceName = x.ResourceName,
                ClientID = x.ClientID,
                UserName = x.UserName,
                DisplayName = x.DisplayName,
                Privs = x.Privs,
                AuthLevel = x.AuthLevel,
                Expiration = x.Expiration,
                EmailNotify = x.EmailNotify,
                PracticeResEmailNotify = x.PracticeResEmailNotify,
                Email = x.Email,
                ClientActive = x.ClientActive
            });
        }

        [Obsolete("Use Model<T>()")]
        public static ReservationItem CreateReservationItem(this Reservation item)
        {
            if (item == null) return null;
            var list = new List<Reservation> { item };
            return CreateReservationItems(list.AsQueryable()).FirstOrDefault();
        }

        [Obsolete("Use Model<T>()")]
        public static IEnumerable<ReservationItem> CreateReservationItems(this IQueryable<Reservation> query)
        {
            if (query == null) return null;

            return query.Select(x => new ReservationItem
            {
                ReservationID = x.ReservationID,
                ResourceID = x.Resource.ResourceID,
                ResourceName = x.Resource.ResourceName,
                ResourceAutoEnd = x.Resource.AutoEnd,
                MinCancelTime = x.Resource.MinCancelTime,
                MinReservTime = x.Resource.MinReservTime,
                ReservFence = x.Resource.ReservFence,
                Granularity = x.Resource.Granularity,
                Offset = x.Resource.Offset,
                GracePeriod = x.Resource.GracePeriod,
                AuthState = x.Resource.AuthState,
                AuthDuration = x.Resource.AuthDuration,
                State = x.Resource.State,
                ProcessTechID = x.Resource.ProcessTech.ProcessTechID,
                ProcessTechName = x.Resource.ProcessTech.ProcessTechName,
                LabID = x.Resource.ProcessTech.Lab.LabID,
                LabName = x.Resource.ProcessTech.Lab.LabName,
                LabDisplayName = x.Resource.ProcessTech.Lab.DisplayName,
                BuildingID = x.Resource.ProcessTech.Lab.Building.BuildingID,
                BuildingName = x.Resource.ProcessTech.Lab.Building.BuildingName,
                ClientID = x.Client.ClientID,
                UserName = x.Client.UserName,
                LName = x.Client.LName,
                FName = x.Client.FName,
                Privs = x.Client.Privs,
                AccountID = x.Account.AccountID,
                AccountName = x.Account.Name,
                ShortCode = x.Account.ShortCode,
                AccountNumber = x.Account.Number,
                OrgID = x.Account.Org.OrgID,
                OrgName = x.Account.Org.OrgName,
                NNINOrg = x.Account.Org.NNINOrg,
                PrimaryOrg = x.Account.Org.PrimaryOrg,
                OrgTypeID = x.Account.Org.OrgType.OrgTypeID,
                OrgTypeName = x.Account.Org.OrgType.OrgTypeName,
                ChargeTypeID = x.Account.Org.OrgType.ChargeType.ChargeTypeID,
                ChargeTypeName = x.Account.Org.OrgType.ChargeType.ChargeTypeName,
                Phone = x.GetPhone(),
                Email = x.GetEmail(),
                ActivityID = x.Activity.ActivityID,
                ActivityName = x.Activity.ActivityName,
                ActivityAccountType = x.Activity.AccountType,
                StartEndAuth = (ClientAuthLevel)x.Activity.StartEndAuth,
                Editable = x.Activity.Editable,
                IsFacilityDownTime = x.Activity.IsFacilityDownTime,
                BeginDateTime = x.BeginDateTime,
                EndDateTime = x.EndDateTime,
                ActualBeginDateTime = x.ActualBeginDateTime,
                ActualEndDateTime = x.ActualEndDateTime,
                ChargeBeginDateTime = x.ChargeBeginDateTime(),
                ChargeEndDateTime = x.ChargeEndDateTime(),
                ClientIDBegin = x.ClientIDBegin,
                ClientBeginLName = x.GetClientBeginLName(),
                ClientBeginFName = x.GetClientBeginFName(),
                ClientIDEnd = x.ClientIDEnd,
                ClientEndLName = x.GetClientEndLName(),
                ClientEndFName = x.GetClientEndFName(),
                CreatedOn = x.CreatedOn,
                LastModifiedOn = x.LastModifiedOn,
                Duration = x.Duration,
                Notes = x.Notes,
                ChargeMultiplier = x.ChargeMultiplier,
                ApplyLateChargePenalty = x.ApplyLateChargePenalty,
                AutoEnd = x.AutoEnd,
                HasProcessInfo = x.HasProcessInfo,
                HasInvitees = x.HasInvitees,
                IsActive = x.IsActive,
                IsStarted = x.IsStarted,
                IsUnloaded = x.IsUnloaded,
                RecurrenceID = x.RecurrenceID,
                GroupID = x.GroupID,
                MaxReservedDuration = x.MaxReservedDuration,
                CancelledDateTime = x.CancelledDateTime,
                KeepAlive = x.KeepAlive,
                OriginalBeginDateTime = x.OriginalBeginDateTime,
                OriginalEndDateTime = x.OriginalEndDateTime,
                OriginalModifiedOn = x.OriginalModifiedOn
            }).ToList();
        }

        [Obsolete("Use CreateModel<T>()")]
        public static ReservationItem CreateReservationItem(this ReservationInfo item)
        {
            if (item == null) return null;
            var list = new List<ReservationInfo> { item };
            return CreateReservationItems(list.AsQueryable()).FirstOrDefault();
        }

        [Obsolete("Use CreateModels<T>()")]
        public static IEnumerable<ReservationItem> CreateReservationItems(this IQueryable<ReservationInfo> query)
        {
            if (query == null) return null;

            return query.Select(x => new ReservationItem
            {
                ReservationID = x.ReservationID,
                ResourceID = x.ResourceID,
                ResourceName = x.ResourceName,
                ResourceAutoEnd = x.ResourceAutoEnd,
                MinCancelTime = x.MinCancelTime,
                MinReservTime = x.MinReservTime,
                ReservFence = x.ReservFence,
                Granularity = x.Granularity,
                Offset = x.Offset,
                GracePeriod = x.GracePeriod,
                AuthState = x.AuthState,
                AuthDuration = x.AuthDuration,
                State = x.State,
                ProcessTechID = x.ProcessTechID,
                ProcessTechName = x.ProcessTechName,
                LabID = x.LabID,
                LabName = x.LabName,
                LabDisplayName = x.LabDisplayName,
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName,
                ClientID = x.ClientID,
                UserName = x.UserName,
                LName = x.LName,
                FName = x.FName,
                Privs = x.Privs,
                AccountID = x.AccountID,
                AccountName = x.AccountName,
                ShortCode = x.ShortCode,
                AccountNumber = x.AccountNumber,
                OrgID = x.OrgID,
                OrgName = x.OrgName,
                NNINOrg = x.NNINOrg,
                PrimaryOrg = x.PrimaryOrg,
                OrgTypeID = x.OrgTypeID,
                OrgTypeName = x.OrgTypeName,
                ChargeTypeID = x.ChargeTypeID,
                ChargeTypeName = x.ChargeTypeName,
                Phone = x.Phone,
                Email = x.Email,
                ActivityID = x.ActivityID,
                ActivityName = x.ActivityName,
                ActivityAccountType = x.ActivityAccountType,
                StartEndAuth = x.StartEndAuth,
                Editable = x.Editable,
                IsFacilityDownTime = x.IsFacilityDownTime,
                BeginDateTime = x.BeginDateTime,
                EndDateTime = x.EndDateTime,
                ActualBeginDateTime = x.ActualBeginDateTime,
                ActualEndDateTime = x.ActualEndDateTime,
                ChargeBeginDateTime = x.ChargeBeginDateTime,
                ChargeEndDateTime = x.ChargeEndDateTime,
                ClientIDBegin = x.ClientIDBegin,
                ClientBeginLName = x.ClientBeginLName,
                ClientBeginFName = x.ClientBeginFName,
                ClientIDEnd = x.ClientIDEnd,
                ClientEndLName = x.ClientEndLName,
                ClientEndFName = x.ClientEndFName,
                CreatedOn = x.CreatedOn,
                LastModifiedOn = x.LastModifiedOn,
                Duration = x.Duration,
                Notes = x.Notes,
                ChargeMultiplier = x.ChargeMultiplier,
                ApplyLateChargePenalty = x.ApplyLateChargePenalty,
                AutoEnd = x.AutoEnd,
                HasProcessInfo = x.HasProcessInfo,
                HasInvitees = x.HasInvitees,
                IsActive = x.IsActive,
                IsStarted = x.IsStarted,
                IsUnloaded = x.IsUnloaded,
                RecurrenceID = x.RecurrenceID,
                GroupID = x.GroupID,
                MaxReservedDuration = x.MaxReservedDuration,
                CancelledDateTime = x.CancelledDateTime,
                KeepAlive = x.KeepAlive,
                OriginalBeginDateTime = x.OriginalBeginDateTime,
                OriginalEndDateTime = x.OriginalEndDateTime,
                OriginalModifiedOn = x.OriginalModifiedOn
            }).ToList();
        }

        public static ReservationItemWithInvitees CreateReservationItemWithInvitees(this Reservation item)
        {
            if (item == null) return null;
            var list = new List<Reservation> { item };
            return CreateReservationItemsWithInvitees(list.AsQueryable()).FirstOrDefault();
        }

        public static IEnumerable<ReservationItemWithInvitees> CreateReservationItemsWithInvitees(this IQueryable<Reservation> query)
        {
            if (query == null) return null;

            return query.Select(x => new ReservationItemWithInvitees
            {
                ReservationID = x.ReservationID,
                ResourceID = x.Resource.ResourceID,
                ResourceName = x.Resource.ResourceName,
                MinCancelTime = x.Resource.MinCancelTime,
                MinReservTime = x.Resource.MinReservTime,
                GracePeriod = x.Resource.GracePeriod,
                ProcessTechID = x.Resource.ProcessTech.ProcessTechID,
                ProcessTechName = x.Resource.ProcessTech.ProcessTechName,
                LabID = x.Resource.ProcessTech.Lab.LabID,
                LabName = x.Resource.ProcessTech.Lab.LabName,
                LabDisplayName = x.Resource.ProcessTech.Lab.DisplayName,
                BuildingID = x.Resource.ProcessTech.Lab.Building.BuildingID,
                BuildingName = x.Resource.ProcessTech.Lab.Building.BuildingName,
                ClientID = x.Client.ClientID,
                UserName = x.Client.UserName,
                LName = x.Client.LName,
                FName = x.Client.FName,
                Privs = x.Client.Privs,
                AccountID = x.Account.AccountID,
                AccountName = x.Account.Name,
                ShortCode = x.Account.ShortCode,
                AccountNumber = x.Account.Number,
                OrgID = x.Account.Org.OrgID,
                OrgName = x.Account.Org.OrgName,
                NNINOrg = x.Account.Org.NNINOrg,
                PrimaryOrg = x.Account.Org.PrimaryOrg,
                OrgTypeID = x.Account.Org.OrgType.OrgTypeID,
                OrgTypeName = x.Account.Org.OrgType.OrgTypeName,
                ChargeTypeID = x.Account.Org.OrgType.ChargeType.ChargeTypeID,
                ChargeTypeName = x.Account.Org.OrgType.ChargeType.ChargeTypeName,
                ActivityID = x.Activity.ActivityID,
                ActivityName = x.Activity.ActivityName,
                ActivityAccountType = x.Activity.AccountType,
                StartEndAuth = (ClientAuthLevel)x.Activity.StartEndAuth,
                Editable = x.Activity.Editable,
                IsFacilityDownTime = x.Activity.IsFacilityDownTime,
                BeginDateTime = x.BeginDateTime,
                EndDateTime = x.EndDateTime,
                ActualBeginDateTime = x.ActualBeginDateTime,
                ActualEndDateTime = x.ActualEndDateTime,
                ChargeBeginDateTime = x.ChargeBeginDateTime(),
                ChargeEndDateTime = x.ChargeEndDateTime(),
                ClientIDBegin = x.ClientIDBegin,
                ClientBeginLName = x.GetClientBeginLName(),
                ClientBeginFName = x.GetClientBeginFName(),
                ClientIDEnd = x.ClientIDEnd,
                ClientEndLName = x.GetClientEndLName(),
                ClientEndFName = x.GetClientEndFName(),
                CreatedOn = x.CreatedOn,
                LastModifiedOn = x.LastModifiedOn,
                Duration = x.Duration,
                Notes = x.Notes,
                ChargeMultiplier = x.ChargeMultiplier,
                ApplyLateChargePenalty = x.ApplyLateChargePenalty,
                AutoEnd = x.AutoEnd,
                HasProcessInfo = x.HasProcessInfo,
                HasInvitees = x.HasInvitees,
                IsActive = x.IsActive,
                IsStarted = x.IsStarted,
                IsUnloaded = x.IsUnloaded,
                RecurrenceID = x.RecurrenceID,
                GroupID = x.GroupID,
                MaxReservedDuration = x.MaxReservedDuration,
                CancelledDateTime = x.CancelledDateTime,
                KeepAlive = x.KeepAlive,
                OriginalBeginDateTime = x.OriginalBeginDateTime,
                OriginalEndDateTime = x.OriginalEndDateTime,
                OriginalModifiedOn = x.OriginalModifiedOn,
                Invitees = DA.Current.Query<ReservationInviteeInfo>().Where(i => i.ReservationID == x.ReservationID).CreateModels<ReservationInviteeItem>()
            }).ToList();
        }

        public static ReservationHistoryItem CreateReservationHistoryItem(this ReservationHistory item)
        {
            if (item == null) return null;
            var list = new List<ReservationHistory> { item };
            return CreateReservationHistoryItems(list.AsQueryable()).FirstOrDefault();
        }

        public static IEnumerable<ReservationHistoryItem> CreateReservationHistoryItems(this IQueryable<ReservationHistory> query)
        {
            if (query == null) return null;

            return query.Select(x => new ReservationHistoryItem
            {
                ReservationHistoryID = x.ReservationHistoryID,
                ReservationID = x.Reservation.ReservationID,
                UserAction = x.UserAction,
                LinkedReservationID = x.LinkedReservationID,
                ActionSource = x.ActionSource,
                ModifiedByClientID = x.ModifiedByClientID,
                ModifiedDateTime = x.ModifiedDateTime,
                AccountID = x.Account.AccountID,
                BeginDateTime = x.BeginDateTime,
                EndDateTime = x.EndDateTime,
                ChargeMultiplier = x.ChargeMultiplier
            }).ToList();
        }
    }
}
