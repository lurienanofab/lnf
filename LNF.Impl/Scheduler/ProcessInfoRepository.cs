using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Scheduler
{
    public class ProcessInfoRepository : RepositoryBase, IProcessInfoRepository
    {
        public ProcessInfoRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IProcessInfo> GetProcessInfos(int resourceId)
        {
            var result = Session.Query<ProcessInfo>().Where(x => x.ResourceID == resourceId && !x.Deleted).ToList();
            return result;
        }

        public IProcessInfoLine GetProcessInfoLine(int processInfoLineId)
        {
            return Session.Get<ProcessInfoLine>(processInfoLineId);
        }

        public IEnumerable<IProcessInfoLine> GetProcessInfoLines(int resourceId)
        {
            var query = Session.Query<ProcessInfo>().Where(x => x.ResourceID == resourceId && !x.Deleted);

            var join = query.Join(Session.Query<ProcessInfoLine>(),
                o => o.ProcessInfoID,
                i => i.ProcessInfoID,
                (o, i) => i);

            var result = join.Where(x => !x.Deleted).ToList();

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

            var result = Session.Query<ReservationProcessInfoInfo>().Where(x => x.ReservationID == reservationId).ToList();
            return result;
        }

        public IEnumerable<IReservationProcessInfo> GetReservationProcessInfos(int[] reservations)
        {
            var result = Session.Query<ReservationProcessInfoInfo>().Where(x => reservations.Contains(x.ReservationID)).ToList();
            return result;
        }

        public void InsertReservationProcessInfo(ReservationProcessInfoItem item)
        {
            // This happens when a new reservation is created.

            ReservationProcessInfo rpi = null;

            if (item.ProcessInfoLineID > 0)
            {
                rpi = new ReservationProcessInfo()
                {
                    ProcessInfoLineID = item.ProcessInfoLineID,
                    ReservationID = item.ReservationID,
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

        public void UpdateReservationProcessInfo(ReservationProcessInfoItem item)
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
                    ProcessInfoLineID = item.ProcessInfoLineID,
                    ReservationID = item.ReservationID
                };

                Session.Save(rpi);

                item.ReservationProcessInfoID = rpi.ReservationProcessInfoID;
            }

            // still here?

            if (item.ProcessInfoLineID != rpi.ProcessInfoLineID)
            {
                // This happens when the ProcessInfo is changed to different ProcessInfoLine
                rpi.ProcessInfoLineID = item.ProcessInfoLineID;
            }

            rpi.Value = item.Value;
            rpi.Special = item.Special;
            rpi.RunNumber = item.RunNumber;
            rpi.ChargeMultiplier = item.ChargeMultiplier;
            rpi.Active = item.Active;

            Session.Update(rpi);
        }

        public IReservationProcessInfo AddReservationProcessInfo(int reservationId, int processInfoLineId, double value, bool special, int runNumber, double chargeMultiplier, bool active)
        {
            var rpi = new ReservationProcessInfo()
            {
                ReservationID = reservationId,
                ProcessInfoLineID = processInfoLineId,
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
            if (insert != null)
            {
                foreach (var item in insert)
                {
                    var pinfo = new ProcessInfo
                    {
                        ResourceID = item.ResourceID,
                        ProcessInfoName = item.ProcessInfoName,
                        ParamName = item.ParamName,
                        ValueName = item.ValueName,
                        Special = item.Special,
                        AllowNone = item.AllowNone,
                        RequireValue = item.RequireValue,
                        RequireSelection = item.RequireSelection,
                        Order = item.Order,
                        Deleted = item.Deleted
                    };

                    Session.Save(pinfo);
                }
            }

            if (update != null)
            {
                foreach (var item in update)
                {
                    var pinfo = Require<ProcessInfo>(item.ProcessInfoID);

                    pinfo.ResourceID = item.ResourceID;
                    pinfo.ProcessInfoName = item.ProcessInfoName;
                    pinfo.ParamName = item.ParamName;
                    pinfo.ValueName = item.ValueName;
                    pinfo.Special = item.Special;
                    pinfo.AllowNone = item.AllowNone;
                    pinfo.Order = item.Order;
                    pinfo.RequireValue = item.RequireValue;
                    pinfo.RequireSelection = item.RequireSelection;
                    pinfo.MaxAllowed = item.MaxAllowed;
                    pinfo.Deleted = item.Deleted;

                    Session.Update(pinfo);
                }
            }

            if (delete != null)
            {
                foreach (var item in delete)
                {
                    var pinfo = Require<ProcessInfo>(item.ProcessInfoID);
                    pinfo.Deleted = true;
                    Session.Update(pinfo);
                }
            }
        }

        public void DeleteProcessInfo(int processInfoId)
        {
            var pinfo = Require<ProcessInfo>(processInfoId);
            Update(null, null, new[] { pinfo });
        }

        public void Update(IEnumerable<IProcessInfoLine> insert, IEnumerable<IProcessInfoLine> update, IEnumerable<IProcessInfoLine> delete)
        {
            if (insert != null)
            {
                foreach (var item in insert)
                {
                    var pil = new ProcessInfoLine
                    {
                        MaxValue = item.MaxValue,
                        MinValue = item.MinValue,
                        Param = item.Param,
                        ProcessInfoID = item.ProcessInfoID,
                        ProcessInfoLineParamID = item.ProcessInfoLineParamID,
                        Deleted = item.Deleted
                    };

                    Session.Save(pil);
                }
            }

            if (update != null)
            {
                foreach (var item in update)
                {
                    var pil = Require<ProcessInfoLine>(item.ProcessInfoLineID);

                    pil.MaxValue = item.MaxValue;
                    pil.MinValue = item.MinValue;
                    pil.Param = item.Param;
                    pil.ProcessInfoID = item.ProcessInfoID;
                    pil.ProcessInfoLineParamID = item.ProcessInfoLineParamID;
                    pil.Deleted = item.Deleted;

                    Session.Update(pil);
                }
            }

            if (delete != null)
            {
                foreach (var item in delete)
                {
                    var pil = Require<ProcessInfoLine>(item.ProcessInfoLineID);
                    pil.Deleted = true;
                    Session.Update(pil);
                }
            }
        }

        public void DeleteProcessInfoLine(int processInfoLineId)
        {
            var pil = Require<ProcessInfoLine>(processInfoLineId);
            Update(null, null, new[] { pil });
        }

        public void Update(IEnumerable<IReservationProcessInfo> insert, IEnumerable<IReservationProcessInfo> update, IEnumerable<IReservationProcessInfo> delete)
        {
            if (insert != null)
            {
                foreach (var item in insert)
                {
                    var rpi = new ReservationProcessInfo
                    {
                        Active = item.Active,
                        ChargeMultiplier = item.ChargeMultiplier,
                        ProcessInfoLineID = item.ProcessInfoLineID,
                        ReservationID = item.ReservationID,
                        RunNumber = item.RunNumber,
                        Special = item.Special,
                        Value = item.Value
                    };

                    Session.Save(rpi);
                }
            }

            if (update != null)
            {
                foreach (var item in update)
                {
                    var rpi = Require<ReservationProcessInfo>(item.ReservationProcessInfoID);

                    rpi.Active = item.Active;
                    rpi.ChargeMultiplier = item.ChargeMultiplier;
                    rpi.ProcessInfoLineID = item.ProcessInfoLineID;
                    rpi.ReservationID = item.ReservationID;
                    rpi.RunNumber = item.RunNumber;
                    rpi.Special = item.Special;
                    rpi.Value = item.Value;

                    Session.Update(rpi);
                }
            }

            if (delete != null)
            {
                foreach (var item in delete)
                {
                    var rpi = Require<ReservationProcessInfo>(item.ReservationProcessInfoID);
                    Session.Delete(rpi);
                }
            }
        }

        public IProcessInfo GetProcessInfo(int processInfoId)
        {
            return Require<ProcessInfo>(processInfoId);
        }

        public IProcessInfo Create(DataRow dr)
        {
            return new ProcessInfo
            {
                ProcessInfoID = dr.Field<int>("ProcessInfoID"),
                ResourceID = dr.Field<int>("ResourceID"),
                ProcessInfoName = dr.Field<string>("ProcessInfoName"),
                ParamName = dr.Field<string>("ParamName"),
                ValueName = dr.Field<string>("ValueName"),
                Special = dr.Field<string>("Special"),
                AllowNone = dr.Field<bool>("AllowNone"),
                Order = dr.Field<int>("Order"),
                RequireValue = dr.Field<bool>("RequireValue"),
                RequireSelection = dr.Field<bool>("RequireSelection"),
                MaxAllowed = dr.Field<int>("MaxAllowed"),
                Deleted = dr.Field<bool>("Deleted")
            };
        }

        public IEnumerable<IProcessInfoLineParam> GetProcessInfoParams(int resourceId)
        {
            return Session.Query<ProcessInfoLineParam>().Where(x => x.ResourceID == resourceId).ToList();
        }

        public IProcessInfoLineParam AddProcessInfoLineParam(int resourceId, string paramName, string paramUnit, int paramType)
        {
            var result = new ProcessInfoLineParam
            {
                ProcessInfoLineParamID = 0,
                ResourceID = resourceId,
                ParameterName = paramName,
                ParameterUnit = paramUnit,
                ParameterType = paramType,
            };

            Session.Save(result);

            return result;
        }
    }
}
