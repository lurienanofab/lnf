using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace LNF.Scheduler
{
    public class Properties
    {
        private static Properties _current;
        private IEnumerable<SchedulerProperty> _props;
        private DateTime _timestamp;

        public double LateChargePenaltyMultiplier { get; set; }
        public double AuthExpWarning { get; set; }
        public Client Admin { get; set; }
        public string ResourceIPPrefix { get; set; }
        public bool AlwaysOnKiosk { get; set; }
        public string SchedulerEmail { get; set; }
        public Account LabAccount { get; set; }
        public ActivityCollection Activities { get; set; }
        public int[] ManualEntryDurationActivities { get; set; }

        private Properties(IList<SchedulerProperty> props)
        {
            IList<Activity> activities = DA.Current.Query<Activity>().ToList();

            _props = props;
            _timestamp = DateTime.Now;
            LateChargePenaltyMultiplier = double.Parse(GetValue("LateChargePenaltyMultiplier"));
            AuthExpWarning = double.Parse(GetValue("AuthExpWarning"));
            Admin = DA.Current.Single<Client>(int.Parse(GetValue("AdminID")));
            ResourceIPPrefix = GetValue("ResourceIPPrefix");
            AlwaysOnKiosk = bool.Parse(GetValue("AlwaysOnKiosk"));
            SchedulerEmail = GetValue("SchedulerEmail");
            LabAccount = DA.Current.Single<Account>(int.Parse(GetValue("LabAccountID")));
            Activities = new ActivityCollection(
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
            ManualEntryDurationActivities = GetValue("ManualEntryDurationActivities").Split(',').Select(int.Parse).ToArray();
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
    }

    public class ActivityCollection : IEnumerable<Activity>
    {
        private Dictionary<string, Activity> _items;

        public ActivityCollection(Activity processing, Activity practice, Activity repair, Activity characterization, Activity staffSupport, Activity schedMaintenance, Activity demonstration, Activity futurePractice, Activity remoteProcessing, Activity fdt)
        {
            _items = new Dictionary<string, Activity>();

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

        public Activity Processing { get { return _items["processing"]; } }

        public Activity Practice { get { return _items["practice"]; } }

        public Activity Repair { get { return _items["repair"]; } }

        public Activity Characterization { get { return _items["characterization"]; } }

        public Activity StaffSupport { get { return _items["staffSupport"]; } }

        public Activity ScheduledMaintenance { get { return _items["schedMaintenance"]; } }

        public Activity Demonstration { get { return _items["demonstration"]; } }

        public Activity FuturePractice { get { return _items["futurePractice"]; } }

        public Activity RemoteProcessing { get { return _items["remoteProcessing"]; } }

        public Activity FacilityDownTime { get { return _items["fdt"]; } }

        public IEnumerator<Activity> GetEnumerator()
        {
            return _items.Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
