using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class Properties
    {
        public double LateChargePenaltyMultiplier
        {
            get { return double.Parse(GetValue("LateChargePenaltyMultiplier")); }
            set { SetValue("LateChargePenaltyMultiplier", value); }
        }

        public double AuthExpWarning
        {
            get { return double.Parse(GetValue("AuthExpWarning")); }
            set { SetValue("AuthExpWarning", value); }
        }

        public IClient Admin
        {
            get { return ServiceProvider.Current.Data.Client.GetClient(int.Parse(GetValue("AdminID"))); }
            set { SetValue("AdminID", value.ClientID); }
        }

        public string ResourceIPPrefix
        {
            get { return GetValue("ResourceIPPrefix"); }
            set { SetValue("ResourceIPPrefix", value); }
        }

        public bool AlwaysOnKiosk
        {
            get { return bool.Parse(GetValue("AlwaysOnKiosk")); }
            set { SetValue("AlwaysOnKiosk", value); }
        }

        public string SchedulerEmail
        {
            get { return GetValue("SchedulerEmail"); }
            set { SetValue("SchedulerEmail", value); }
        }

        public IAccount LabAccount
        {
            get { return CacheManager.Current.GetAccount(int.Parse(GetValue("LabAccountID"))); }
            set { SetValue("LabAccountID", value.AccountID); }
        }

        public ActivityCollection Activities => GetActivityCollection();

        public int[] ManualEntryDurationActivities
        {
            get { return GetValue("ManualEntryDurationActivities").Split(',').Select(int.Parse).ToArray(); }
            set { SetValue("ManualEntryDurationActivities", string.Join(",", value)); }
        }

        private Properties(IEnumerable<SchedulerPropertyItem> props)
        {
            _props = props;
            Timestamp = DateTime.Now;
        }

        public DateTime Timestamp{get;}

        static Properties()
        {
            Current = new Properties(CacheManager.Current.SchedulerProperties());
        }

        public static Properties Current { get; }

        private IEnumerable<SchedulerPropertyItem> _props;
        private ActivityCollection _activities;

        public void Save()
        {
            SetValue("LateChargePenaltyMultiplier", LateChargePenaltyMultiplier);
            SetValue("AuthExpWarning", AuthExpWarning);
            SetValue("AdminID", Admin.ClientID);
            SetValue("ResourceIPPrefix", ResourceIPPrefix);
            SetValue("AlwaysOnKiosk", AlwaysOnKiosk ? "true" : "false");
            SetValue("SchedulerEmail", SchedulerEmail);
            SetValue("LabAccountID", LabAccount.AccountID);
            SetValue("Activities.Processing", Activities.Processing.ActivityID);
            SetValue("Activities.Practice", Activities.Practice.ActivityID);
            SetValue("Activities.Repair", Activities.Repair.ActivityID);
            SetValue("Activities.Characterization", Activities.Characterization.ActivityID);
            SetValue("Activities.StaffSupport", Activities.StaffSupport.ActivityID);
            SetValue("Activities.ScheduledMaintenance", Activities.ScheduledMaintenance.ActivityID);
            SetValue("Activities.Demonstration", Activities.Demonstration.ActivityID);
            SetValue("Activities.FuturePractice", Activities.FuturePractice.ActivityID);
            SetValue("Activities.RemoteProcessing", Activities.RemoteProcessing.ActivityID);
            SetValue("Activities.FacilityDownTime", Activities.FacilityDownTime.ActivityID);
            SetValue("ManualEntryDurationActivities", string.Join(",", ManualEntryDurationActivities));
        }

        private string GetValue(string key)
        {
            var sp = _props.FirstOrDefault(x => x.PropertyName == key);

            if (sp == null)
                throw new Exception(string.Format("Property not found: {0}", key));

            return sp.PropertyValue;
        }

        private void SetValue(string key, object value)
        {
            var sp = _props.FirstOrDefault(x => x.PropertyName == key);

            if (sp == null)
                throw new Exception(string.Format("Property not found: {0}", key));

            sp.PropertyValue = value.ToString();
        }

        private ActivityCollection GetActivityCollection()
        {
            var activities = CacheManager.Current.Activities();

            if (_activities == null)
            {
                _activities = new ActivityCollection(
                   processing: activities.First(x => x.ActivityID == int.Parse(GetValue("Activities.Processing"))),
                   practice: activities.First(x => x.ActivityID == int.Parse(GetValue("Activities.Practice"))),
                   repair: activities.First(x => x.ActivityID == int.Parse(GetValue("Activities.Repair"))),
                   characterization: activities.First(x => x.ActivityID == int.Parse(GetValue("Activities.Characterization"))),
                   staffSupport: activities.First(x => x.ActivityID == int.Parse(GetValue("Activities.StaffSupport"))),
                   schedMaintenance: activities.First(x => x.ActivityID == int.Parse(GetValue("Activities.ScheduledMaintenance"))),
                   demonstration: activities.First(x => x.ActivityID == int.Parse(GetValue("Activities.Demonstration"))),
                   futurePractice: activities.First(x => x.ActivityID == int.Parse(GetValue("Activities.FuturePractice"))),
                   remoteProcessing: activities.First(x => x.ActivityID == int.Parse(GetValue("Activities.RemoteProcessing"))),
                   fdt: activities.First(x => x.ActivityID == int.Parse(GetValue("Activities.FacilityDownTime"))));
            }

            return _activities;
        }
    }

    public class ActivityCollection : IEnumerable<IActivity>
    {
        private Dictionary<string, IActivity> _items;

        public ActivityCollection(IActivity processing, IActivity practice, IActivity repair, IActivity characterization, IActivity staffSupport, IActivity schedMaintenance, IActivity demonstration, IActivity futurePractice, IActivity remoteProcessing, IActivity fdt)
        {
            _items = new Dictionary<string, IActivity>
            {
                { "processing", processing },
                { "practice", practice },
                { "repair", repair },
                { "characterization", characterization },
                { "staffSupport", staffSupport },
                { "schedMaintenance", schedMaintenance },
                { "demonstration", demonstration },
                { "futurePractice", futurePractice },
                { "remoteProcessing", remoteProcessing },
                { "fdt", fdt }
            };
        }

        public IActivity Processing
        {
            get { return _items["processing"]; }
            set { _items["processing"] = value; }
        }

        public IActivity Practice
        {
            get { return _items["practice"]; }
            set { _items["practice"] = value; }
        }

        public IActivity Repair
        {
            get { return _items["repair"]; }
            set { _items["repair"] = value; }
        }

        public IActivity Characterization
        {
            get { return _items["characterization"]; }
            set { _items["characterization"] = value; }
        }

        public IActivity StaffSupport
        {
            get { return _items["staffSupport"]; }
            set { _items["staffSupport"] = value; }
        }

        public IActivity ScheduledMaintenance
        {
            get { return _items["schedMaintenance"]; }
            set { _items["schedMaintenance"] = value; }
        }

        public IActivity Demonstration
        {
            get { return _items["demonstration"]; }
            set { _items["demonstration"] = value; }
        }

        public IActivity FuturePractice
        {
            get { return _items["futurePractice"]; }
            set { _items["futurePractice"] = value; }
        }

        public IActivity RemoteProcessing
        {
            get { return _items["remoteProcessing"]; }
            set { _items["remoteProcessing"] = value; }
        }

        public IActivity FacilityDownTime
        {
            get { return _items["fdt"]; }
            set { _items["fdt"] = value; }
        }

        public IEnumerator<IActivity> GetEnumerator()
        {
            return _items.Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
