using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ProcessInfos
    {
        private IProvider _provider;

        public ProcessInfos(IProvider provider)
        {
            _provider = provider;
        }

        public static List<ReservationProcessInfoItem> CreateReservationProcessInfoItems(IEnumerable<IReservationProcessInfo> items)
        {
            var result = new List<ReservationProcessInfoItem>();
            foreach (var i in items)
            {
                result.Add(CreateReservationProcessInfoItem(i));
            }
            return result;
        }

        public static ReservationProcessInfoItem CreateReservationProcessInfoItem(IReservationProcessInfo item)
        {
            return new ReservationProcessInfoItem
            {
                ReservationProcessInfoID = item.ReservationProcessInfoID,
                ProcessInfoID = item.ProcessInfoID,
                ReservationID = item.ReservationID,
                ProcessInfoLineID = item.ProcessInfoLineID,
                Value = item.Value,
                Special = item.Special,
                RunNumber = item.RunNumber,
                ChargeMultiplier = item.ChargeMultiplier,
                Active = item.Active
            };
        }

        public IEnumerable<ProcessInfoModel> AddProcessInfo(IProcessInfo model)
        {
            int resourceId = model.ResourceID;
            var pinfos = _provider.Scheduler.ProcessInfo.GetProcessInfos(resourceId).OrderBy(x => x.Order).ToList();
            var maxOrder = pinfos.Max(x => x.Order);
            model.Order = maxOrder + 1;

            _provider.Scheduler.ProcessInfo.Update(new[] { model }, null, null);

            return GetProcessInfos(resourceId);
        }

        public IEnumerable<ProcessInfoModel> UpdateProcessInfo(IProcessInfo model)
        {
            _provider.Scheduler.ProcessInfo.Update(null, new[] { model }, null);
            return GetProcessInfos(model.ResourceID);
        }

        public IEnumerable<IProcessInfo> MoveUp(int resourceId, int processInfoId)
        {
            var sort = new ProcessInfoSort(_provider.Scheduler.ProcessInfo.GetProcessInfos(resourceId));
            sort.MoveUp(processInfoId);
            var list = sort.ToList();
            _provider.Scheduler.ProcessInfo.Update(null, list, null);
            return GetProcessInfos(resourceId);
        }

        public IEnumerable<IProcessInfo> MoveDown(int resourceId, int processInfoId)
        {
            var sort = new ProcessInfoSort(_provider.Scheduler.ProcessInfo.GetProcessInfos(resourceId));
            sort.MoveDown(processInfoId);
            var list = sort.ToList();
            _provider.Scheduler.ProcessInfo.Update(null, list, null);
            return GetProcessInfos(resourceId);
        }

        public IEnumerable<ProcessInfoModel> DeleteProcessInfo(int resourceId, int processInfoId)
        {
            _provider.Scheduler.ProcessInfo.DeleteProcessInfo(processInfoId);
            return GetProcessInfos(resourceId);
        }

        public IEnumerable<ProcessInfoModel> GetProcessInfos(int resourceId)
        {
            var result = new List<ProcessInfoModel>();
            var infos = _provider.Scheduler.Resource.GetProcessInfo(resourceId).OrderBy(x => x.Order).ToList();
            var lines = _provider.Scheduler.Resource.GetProcessInfoLines(resourceId);

            foreach (var pinfo in infos)
            {
                result.Add(new ProcessInfoModel
                {
                    ProcessInfoID = pinfo.ProcessInfoID,
                    ResourceID = pinfo.ResourceID,
                    ProcessInfoName = pinfo.ProcessInfoName,
                    ParamName = pinfo.ParamName,
                    ValueName = pinfo.ValueName,
                    Special = pinfo.Special,
                    AllowNone = pinfo.AllowNone,
                    Order = pinfo.Order,
                    RequireValue = pinfo.RequireValue,
                    RequireSelection = pinfo.RequireSelection,
                    MaxAllowed = pinfo.MaxAllowed,
                    Lines = GetProcessInfoLines(pinfo.ProcessInfoID, lines)
                });
            }

            return result;
        }

        public IEnumerable<IProcessInfoLine> GetProcessInfoLines(int processInfoId, IEnumerable<IProcessInfoLine> lines)
        {
            return lines.Where(x => x.ProcessInfoID == processInfoId).OrderBy(x => x.Param).ToList();
        }

        private IProcessInfoLineParam GetProcessInfoLineParam(int resourceId, string param)
        {
            var lineParams = _provider.Scheduler.ProcessInfo.GetProcessInfoParams(resourceId);

            var result = lineParams.FirstOrDefault(x => x.ParameterName == param);

            if (result == null)
                result = _provider.Scheduler.ProcessInfo.AddProcessInfoLineParam(resourceId, param, null, 0);

            return result;
        }

        public IEnumerable<IProcessInfoLine> AddProcessInfoLine(int resourceId, IProcessInfoLine model)
        {
            var p = GetProcessInfoLineParam(resourceId, model.Param);
            model.ProcessInfoLineParamID = p.ProcessInfoLineParamID;

            _provider.Scheduler.ProcessInfo.Update(new[] { model }, null, null);

            var lines = _provider.Scheduler.Resource.GetProcessInfoLines(resourceId);

            return GetProcessInfoLines(model.ProcessInfoID, lines);
        }

        public IEnumerable<IProcessInfoLine> UpdateProcessInfoLine(int resourceId, IProcessInfoLine model)
        {
            var p = GetProcessInfoLineParam(resourceId, model.Param);
            model.ProcessInfoLineParamID = p.ProcessInfoLineParamID;

            _provider.Scheduler.ProcessInfo.Update(null, new[] { model }, null);

            var lines = _provider.Scheduler.Resource.GetProcessInfoLines(resourceId);

            return GetProcessInfoLines(model.ProcessInfoID, lines);
        }

        public IEnumerable<IProcessInfoLine> DeleteProcessInfoLine(int resourceId, int processInfoId, int processInfoLineId)
        {
            _provider.Scheduler.ProcessInfo.DeleteProcessInfoLine(processInfoLineId);
            var lines = _provider.Scheduler.ProcessInfo.GetProcessInfoLines(resourceId);
            return GetProcessInfoLines(processInfoId, lines);
        }
    }
}
