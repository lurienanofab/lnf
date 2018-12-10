using LNF.Models.Scheduler;
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

        public static ReservationItem CreateReservationItem(this Reservation item)
        {
            if (item == null) return null;
            var list = new List<Reservation> { item };
            return CreateReservationItems(list.AsQueryable()).FirstOrDefault();
        }

        public static IEnumerable<ReservationItem> CreateReservationItems(this IQueryable<Reservation> query)
        {
            if (query == null) return null;

            return query.Select(x => new ReservationWithInvitees
            {
                ReservationID = x.ReservationID,
                ResourceID = x.Resource.ResourceID,
                ResourceName = x.Resource.ResourceName,
                MinCancelTime = x.Resource.MinCancelTime,
                MinReservTime = x.Resource.MinReservTime,
                ProcessTechID = x.Resource.ProcessTech.ProcessTechID,
                LabID = x.Resource.ProcessTech.Lab.LabID,
                BuildingID = x.Resource.ProcessTech.Lab.Building.BuildingID,
                ClientID = x.Client.ClientID,
                UserName = x.Client.UserName,
                LName = x.Client.LName,
                FName = x.Client.FName,
                Privs = x.Client.Privs,
                AccountID = x.Account.AccountID,
                AccountName = x.Account.Name,
                ShortCode = x.Account.ShortCode,
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
                ClientIDEnd = x.ClientIDEnd,
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

        public static ReservationWithInvitees CreateReservationWithInvitees(this Reservation item)
        {
            if (item == null) return null;
            var list = new List<Reservation> { item };
            return CreateReservationsWithInvitees(list.AsQueryable()).FirstOrDefault();
        }

        public static IEnumerable<ReservationWithInvitees> CreateReservationsWithInvitees(this IQueryable<Reservation> query)
        {
            if (query == null) return null;

            return query.Select(x => new ReservationWithInvitees
            {
                ReservationID = x.ReservationID,
                ResourceID = x.Resource.ResourceID,
                ResourceName = x.Resource.ResourceName,
                MinCancelTime = x.Resource.MinCancelTime,
                MinReservTime = x.Resource.MinReservTime,
                ProcessTechID = x.Resource.ProcessTech.ProcessTechID,
                LabID = x.Resource.ProcessTech.Lab.LabID,
                BuildingID = x.Resource.ProcessTech.Lab.Building.BuildingID,
                ClientID = x.Client.ClientID,
                UserName = x.Client.UserName,
                LName = x.Client.LName,
                FName = x.Client.FName,
                Privs = x.Client.Privs,
                AccountID = x.Account.AccountID,
                AccountName = x.Account.Name,
                ShortCode = x.Account.ShortCode,
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
                ClientIDEnd = x.ClientIDEnd,
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
                Invitees = CreateReservationInviteeItems(DA.Current.Query<ReservationInvitee>().Where(i => i.Reservation.ReservationID == x.ReservationID))
            }).ToList();
        }

        public static ReservationInviteeItem CreateReservationInviteeItems(this ReservationInvitee item)
        {
            if (item == null) return null;
            var list = new List<ReservationInvitee> { item };
            return CreateReservationInviteeItems(list.AsQueryable()).FirstOrDefault();
        }

        public static IEnumerable<ReservationInviteeItem> CreateReservationInviteeItems(this IQueryable<ReservationInvitee> query)
        {
            if (query == null) return null;

            return query.Select(x => new ReservationInviteeItem
            {
                ReservationID = x.Reservation.ReservationID,
                ClientID = x.Invitee.ClientID,
                LName = x.Invitee.LName,
                FName = x.Invitee.FName
            }).ToList();
        }
    }
}
