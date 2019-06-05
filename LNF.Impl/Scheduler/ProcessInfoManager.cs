using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Scheduler
{
    public class ProcessInfoManager : ManagerBase, IProcessInfoManager
    {
        public ProcessInfoManager(IProvider provider) : base(provider) { }

        public IEnumerable<IProcessInfo> GetProcessInfos(int resourceId)
        {
            var result = Session.Query<ProcessInfo>()
                .Where(x => x.Resource.ResourceID == resourceId)
                .Select(x => new ProcessInfoItem
                {
                    ProcessInfoID = x.ProcessInfoID,
                    ResourceID = x.Resource.ResourceID,
                    ResourceName = x.Resource.ResourceName,
                    ProcessInfoName = x.ProcessInfoName,
                    ParamName = x.ParamName,
                    ValueName = x.ValueName,
                    Special = x.Special,
                    AllowNone = x.AllowNone,
                    Order = x.Order,
                    RequireValue = x.RequireValue,
                    RequireSelection = x.RequireSelection
                }).ToList();

            return result;
        }

        public IProcessInfoLine GetProcessInfoLine(int processInfoLineId)
        {
            var pil = Session.Single<ProcessInfoLine>(processInfoLineId);
            var pi = Session.Single<ProcessInfo>(pil.ProcessInfoID);

            return new ProcessInfoLineItem
            {
                ProcessInfoLineID = pil.ProcessInfoLineID,
                ProcessInfoID = pi.ProcessInfoID,
                Param = pil.Param,
                MinValue = pil.MinValue,
                MaxValue = pil.MaxValue,
                ProcessInfoLineParamID = pil.ProcessInfoLineParam.ProcessInfoLineParamID,
                ResourceID = pi.Resource.ResourceID,
                ResourceName = pi.Resource.ResourceName,
                ParameterName = pil.ProcessInfoLineParam.ParameterName,
                ParameterType = pil.ProcessInfoLineParam.ParameterType
            };
        }

        public IEnumerable<IProcessInfoLine> GetProcessInfoLines(int resourceId)
        {
            var query = Session.Query<ProcessInfo>().Where(x => x.Resource.ResourceID == resourceId);

            var result = query.Join(Session.Query<ProcessInfoLine>(),
                o => o.ProcessInfoID,
                i => i.ProcessInfoID,
                (o, i) => new ProcessInfoLineItem
                {
                    ProcessInfoLineID = i.ProcessInfoLineID,
                    ProcessInfoID = o.ProcessInfoID,
                    Param = i.Param,
                    MinValue = i.MinValue,
                    MaxValue = i.MaxValue,
                    ProcessInfoLineParamID = i.ProcessInfoLineParam.ProcessInfoLineParamID,
                    ResourceID = o.Resource.ResourceID,
                    ResourceName = o.Resource.ResourceName,
                    ParameterName = i.ProcessInfoLineParam.ParameterName,
                    ParameterType = i.ProcessInfoLineParam.ParameterType
                }).ToList();

            return result;
        }

        public IEnumerable<IReservationProcessInfo> GetReservationProcessInfos(int reservationId)
        {
            /*
             * sselScheduler.dbo.procReservationProcessInfoSelect
             * 
                SELECT PI.ProcessInfoID, PI.ProcessInfoName, PIL.Param, par.ParameterName, RPI.*
                FROM dbo.ReservationProcessInfo RPI, dbo.ProcessInfoLine PIL, ProcessInfoLineParam par,  dbo.ProcessInfo PI
                WHERE RPI.ProcessInfoLineID = PIL.ProcessInfoLineID
                AND PIL.ProcessInfoID = PI.ProcessInfoID
                AND PIL.ProcessInfoLineParamID = par.ProcessInfoLineParamID 
                AND ReservationID = @ReservationID 
            */

            var query = Session.Query<ReservationProcessInfo>().Where(x => x.Reservation.ReservationID == reservationId);

            var result = query.Join(Session.Query<ProcessInfo>(),
                o => o.ProcessInfoLine.ProcessInfoID,
                i => i.ProcessInfoID,
                (o, i) => new Models.Scheduler.ReservationProcessInfoItem
                {
                    ProcessInfoID = i.ProcessInfoID,
                    ProcessInfoName = i.ProcessInfoName,
                    Param = o.ProcessInfoLine.Param,
                    ParameterName = o.ProcessInfoLine.ProcessInfoLineParam.ParameterName,
                    ReservationProcessInfoID = o.ReservationProcessInfoID,
                    ProcessInfoLineID = o.ProcessInfoLine.ProcessInfoLineID,
                    ProcessInfoLineParamID = o.ProcessInfoLine.ProcessInfoLineParam.ProcessInfoLineParamID,
                    ReservationID = o.Reservation.ReservationID,
                    Value = o.Value,
                    Special = o.Special,
                    RunNumber = o.RunNumber,
                    ChargeMultiplier = o.ChargeMultiplier,
                    Active = o.Active
                }).ToList();

            return result;
        }

        public void InsertReservationProcessInfo(IReservationProcessInfo item)
        {
            // This happens when a new reservation is created.

            if (item.ProcessInfoLineID > 0)
            {
                var rpi = new ReservationProcessInfo()
                {
                    ProcessInfoLine = Session.Single<ProcessInfoLine>(item.ProcessInfoLineID),
                    Reservation = Session.Single<Reservation>(item.ReservationID),
                    Value = item.Value,
                    Special = item.Special,
                    RunNumber = item.RunNumber,
                    ChargeMultiplier = item.ChargeMultiplier,
                    Active = item.Active
                };

                Session.Insert(rpi);

                item.ReservationProcessInfoID = rpi.ReservationProcessInfoID;
            }
        }

        public void UpdateReservationProcessInfo(IReservationProcessInfo item)
        {
            var rpi = Session.Single<ReservationProcessInfo>(item.ReservationProcessInfoID);

            if (rpi != null)
            {
                if (item.ProcessInfoLineID == 0)
                {
                    // Delete the record.
                    // This happens when an existing record is changed to "None" (i.e. removed).
                    Session.Delete(rpi);
                    return;
                }
            }
            else
            {
                // Insert a new record if it doesn't exist.
                // This happens when a reservation is modified and an addtional process info is selected.
                rpi = new ReservationProcessInfo()
                {
                    ProcessInfoLine = Session.Single<ProcessInfoLine>(item.ProcessInfoLineID),
                    Reservation = Session.Single<Reservation>(item.ReservationID)
                };

                Session.Insert(rpi);

                item.ReservationProcessInfoID = rpi.ReservationProcessInfoID;
            }

            // still here?

            if (item.ProcessInfoLineID != rpi.ProcessInfoLine.ProcessInfoLineID)
            {
                // This happens when the ProcessInfo is changed to different ProcessInfoLine
                var pil = Session.Single<ProcessInfoLine>(item.ProcessInfoLineID);
                rpi.ProcessInfoLine = pil;
            }

            rpi.Value = item.Value;
            rpi.Special = item.Special;
            rpi.RunNumber = item.RunNumber;
            rpi.ChargeMultiplier = item.ChargeMultiplier;
            rpi.Active = item.Active;
        }
    }
}
