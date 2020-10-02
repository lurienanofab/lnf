using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Scheduler
{
    public class ProcessInfoRepository : RepositoryBase, IProcessInfoRepository
    {
        public ProcessInfoRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IProcessInfo> GetProcessInfos(int resourceId)
        {
            //'DA.Current.Query(Of ProcessInfo)().Where(Function(x) x.Resource.ResourceID = Resource.ResourceID).ToList()

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
            var pil = Session.Get<ProcessInfoLine>(processInfoLineId);
            var pi = Session.Get<ProcessInfo>(pil.ProcessInfoID);

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
            //var query = Session.Query<ProcessInfo>().Where(x => x.ProcessInfoID == processInfoId);

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
                (o, i) => new ReservationProcessInfoItem
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
                    ProcessInfoLine = Session.Get<ProcessInfoLine>(item.ProcessInfoLineID),
                    Reservation = Session.Get<Reservation>(item.ReservationID),
                    Value = item.Value,
                    Special = item.Special,
                    RunNumber = item.RunNumber,
                    ChargeMultiplier = item.ChargeMultiplier,
                    Active = item.Active
                };

                Session.Save(rpi);

                item.ReservationProcessInfoID = rpi.ReservationProcessInfoID;
            }
        }

        public void UpdateReservationProcessInfo(IReservationProcessInfo item)
        {
            var rpi = Session.Get<ReservationProcessInfo>(item.ReservationProcessInfoID);

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
                    ProcessInfoLine = Session.Get<ProcessInfoLine>(item.ProcessInfoLineID),
                    Reservation = Session.Get<Reservation>(item.ReservationID)
                };

                Session.Save(rpi);

                item.ReservationProcessInfoID = rpi.ReservationProcessInfoID;
            }

            // still here?

            if (item.ProcessInfoLineID != rpi.ProcessInfoLine.ProcessInfoLineID)
            {
                // This happens when the ProcessInfo is changed to different ProcessInfoLine
                var pil = Session.Get<ProcessInfoLine>(item.ProcessInfoLineID);
                rpi.ProcessInfoLine = pil;
            }

            rpi.Value = item.Value;
            rpi.Special = item.Special;
            rpi.RunNumber = item.RunNumber;
            rpi.ChargeMultiplier = item.ChargeMultiplier;
            rpi.Active = item.Active;
        }

        public IReservationProcessInfo AddReservationProcessInfo(int reservationId, int processInfoLineId, double value, bool special, int runNumber, double chargeMultiplier, bool active)
        {
            var rpi = new ReservationProcessInfo()
            {
                Reservation = Require<Reservation>(reservationId),
                ProcessInfoLine = Require<ProcessInfoLine>(processInfoLineId),
                Value = value,
                Special = special,
                RunNumber = runNumber,
                ChargeMultiplier = chargeMultiplier,
                Active = active
            };

            Session.Save(rpi);

            return rpi.CreateModel<IReservationProcessInfo>();
        }

        public void Update(IEnumerable<IProcessInfo> insert, IEnumerable<IProcessInfo> update, IEnumerable<IProcessInfo> delete)
        {
            foreach (var item in insert)
            {
                var processInfo = new ProcessInfo
                {
                    Resource = Require<Resource>(item.ResourceID),
                    ProcessInfoName = item.ProcessInfoName,
                    ParamName = item.ParamName,
                    ValueName = item.ValueName,
                    Special = item.Special,
                    AllowNone = item.AllowNone,
                    RequireValue = item.RequireValue,
                    RequireSelection = item.RequireSelection,
                    Order = item.Order
                };

                Session.Save(processInfo);
            }

            foreach (var item in update)
            {
                var processInfo = Require<ProcessInfo>(item.ProcessInfoID);

                processInfo.Resource = Require<Resource>(item.ResourceID);
                processInfo.ProcessInfoName = item.ProcessInfoName;
                processInfo.ParamName = item.ParamName;
                processInfo.ValueName = item.ValueName;
                processInfo.Special = item.Special;
                processInfo.AllowNone = item.AllowNone;
                processInfo.RequireValue = item.RequireValue;
                processInfo.RequireSelection = item.RequireSelection;
                processInfo.Order = item.Order;

                Session.Update(processInfo);
            }

            foreach (var item in delete)
            {
                var processInfo = Require<ProcessInfo>(item.ProcessInfoID);
                Session.Delete(processInfo);
            }
        }

        public void Update(IEnumerable<IProcessInfoLine> insert, IEnumerable<IProcessInfoLine> update, IEnumerable<IProcessInfoLine> delete)
        {
            foreach (var item in insert)
            {
                var pil = new ProcessInfoLine
                {
                    MaxValue = item.MaxValue,
                    MinValue = item.MinValue,
                    Param = item.Param,
                    ProcessInfoID = item.ProcessInfoID,
                    ProcessInfoLineParam = Require<ProcessInfoLineParam>(item.ProcessInfoLineParamID)
                };

                Session.Save(pil);
            }

            foreach (var item in update)
            {
                var pil = Require<ProcessInfoLine>(item.ProcessInfoLineID);

                pil.MaxValue = item.MaxValue;
                pil.MinValue = item.MinValue;
                pil.Param = item.Param;
                pil.ProcessInfoID = item.ProcessInfoID;
                pil.ProcessInfoLineParam = Require<ProcessInfoLineParam>(item.ProcessInfoLineParamID);

                Session.Update(pil);
            }

            foreach (var item in delete)
            {
                var pil = Require<ProcessInfoLine>(item.ProcessInfoLineID);
                Session.Delete(pil);
            }
        }

        public void Update(IEnumerable<IReservationProcessInfo> insert, IEnumerable<IReservationProcessInfo> update, IEnumerable<IReservationProcessInfo> delete)
        {
            foreach (var item in insert)
            {
                var rpi = new ReservationProcessInfo
                {
                    Active = item.Active,
                    ChargeMultiplier = item.ChargeMultiplier,
                    ProcessInfoLine = Require<ProcessInfoLine>(item.ProcessInfoLineID),
                    Reservation = Require<Reservation>(item.ReservationID),
                    RunNumber = item.RunNumber,
                    Special = item.Special,
                    Value = item.Value
                };

                Session.Save(rpi);
            }

            foreach (var item in update)
            {
                var rpi = Require<ReservationProcessInfo>(item.ReservationProcessInfoID);

                rpi.Active = item.Active;
                rpi.ChargeMultiplier = item.ChargeMultiplier;
                rpi.ProcessInfoLine = Require<ProcessInfoLine>(item.ProcessInfoLineID);
                rpi.Reservation = Require<Reservation>(item.ReservationID);
                rpi.RunNumber = item.RunNumber;
                rpi.Special = item.Special;
                rpi.Value = item.Value;

                Session.Update(rpi);
            }

            foreach(var item in delete)
            {
                var rpi = Require<ReservationProcessInfo>(item.ReservationProcessInfoID);
                Session.Delete(rpi);
            }
        }

        public IProcessInfo GetProcessInfo(int processInfoId)
        {
            var pi = Require<ProcessInfo>(processInfoId);

            var result = new ProcessInfoItem
            {
                AllowNone = pi.AllowNone,
                Order = pi.Order,
                ParamName = pi.ParamName,
                ProcessInfoID = pi.ProcessInfoID,
                ProcessInfoName = pi.ProcessInfoName,
                RequireSelection = pi.RequireSelection,
                RequireValue = pi.RequireValue,
                ResourceID = pi.Resource.ResourceID,
                ResourceName = pi.Resource.ResourceName,
                Special = pi.Special,
                ValueName = pi.ValueName
            };

            return result;
        }
    }
}
