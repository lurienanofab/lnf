using LNF.Cache;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LNF.Data;

namespace LNF.Scheduler
{
    public class Properties
    {
        private static Properties _current;
        private IEnumerable<SchedulerProperty> _props;
        private DateTime _timestamp;
        private ActivityCollection _activities;

        public double LateChargePenaltyMultiplier
        {
            get { return double.Parse(GetValue("LateChargePenaltyMultiplier")); }
            set { SetValue("LateChargePenaltyMultiplier", value); }
        }

        public double AuthExpWarning
        {
            get{ return double.Parse(GetValue("AuthExpWarning")); }
            set{ SetValue("AuthExpWarning", value); }
        }

        public ClientItem Admin
        {
            get { return CacheManager.Current.GetClient(int.Parse(GetValue("AdminID"))); }
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
        public AccountItem LabAccount
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

        private Properties(IList<SchedulerProperty> props)
        {
            _props = props;
            _timestamp = DateTime.Now;
        }

        public DateTime Timestamp()
        {
            return _timestamp;
        }

        public static void Load()
        {
            _current = new Properties(DA.Current.Query<SchedulerProperty>().ToList());
        }

        public static Properties Current
        {
            get
            {
                if (_current == null) Load();
                return _current;
            }
        }

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
            SchedulerProperty sp = _props.FirstOrDefault(x => x.PropertyName == key);

            if (sp == null)
                throw new Exception(string.Format("Property not found: {0}", key));

            return sp.PropertyValue;
        }

        private void SetValue(string key, object value)
        {
            SchedulerProperty sp = _props.FirstOrDefault(x => x.PropertyName == key);

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

    public class ActivityCollection : IEnumerable<ActivityModel>
    {
        private Dictionary<string, ActivityModel> _items;

        public ActivityCollection(ActivityModel processing, ActivityModel practice, ActivityModel repair, ActivityModel characterization, ActivityModel staffSupport, ActivityModel schedMaintenance, ActivityModel demonstration, ActivityModel futurePractice, ActivityModel remoteProcessing, ActivityModel fdt)
        {
            _items = new Dictionary<string, ActivityModel>();

            _items.Add("processing", processing);
            _items.Add("practice", practice);
            _items.Add("repair", repair);
            _items.Add("characterization", characterization);
            _items.Add("staffSupport", staffSupport);
            _items.Add("schedMaintenance", schedMaintenance);
            _items.Add("demonstration", demonstration);
            _items.Add("futurePractice", futurePractice);
            _items.Add("remoteProcessing", remoteProcessing);
            _items.Add("fdt", fdt);
        }

        public ActivityModel Processing
        {
            get { return _items["processing"]; }
            set { _items["processing"] = value; }
        }

        public ActivityModel Practice
        {
            get { return _items["practice"]; }
            set { _items["practice"] = value; }
        }

        public ActivityModel Repair
        {
            get { return _items["repair"]; }
            set { _items["repair"] = value; }
        }

        public ActivityModel Characterization
        {
            get { return _items["characterization"]; }
            set { _items["characterization"] = value; }
        }

        public ActivityModel StaffSupport
        {
            get { return _items["staffSupport"]; }
            set { _items["staffSupport"] = value; }
        }

        public ActivityModel ScheduledMaintenance
        {
            get { return _items["schedMaintenance"]; }
            set { _items["schedMaintenance"] = value; }
        }

        public ActivityModel Demonstration
        {
            get { return _items["demonstration"]; }
            set { _items["demonstration"] = value; }
        }

        public ActivityModel FuturePractice
        {
            get { return _items["futurePractice"]; }
            set { _items["futurePractice"] = value; }
        }

        public ActivityModel RemoteProcessing
        {
            get { return _items["remoteProcessing"]; }
            set { _items["remoteProcessing"] = value; }
        }

        public ActivityModel FacilityDownTime
        {
            get { return _items["fdt"]; }
            set { _items["fdt"] = value; }
        }

        public IEnumerator<ActivityModel> GetEnumerator()
        {
            return _items.Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
