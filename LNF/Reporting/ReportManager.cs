using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LNF.Reporting
{
    public static class ReportManager
    {
        private readonly static List<Type> _list;

        static ReportManager()
        {
            _list = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                Type[] types = a.GetTypes();
                foreach (Type t in types)
                {
                    if (typeof(IReport).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    {
                        _list.Add(t);
                    }
                }
            }
        }

        public static IList<IReport> RetrieveReports()
        {
            List<IReport> result = new List<IReport>();
            List<Type> reportTypes = _list.Where(x => typeof(IReport).IsAssignableFrom(x)).ToList();
            foreach (Type t in reportTypes)
            {
                IReport report = (IReport)Activator.CreateInstance(t);
                result.Add(report);
            }
            return result;
        }

        public static IList<IReport<T>> RetrieveReports<T>() where T : IReportCriteria
        {
            List<IReport<T>> result = new List<IReport<T>>();
            List<Type> reportTypes = _list.Where(x => typeof(IReport<T>).IsAssignableFrom(x)).ToList();
            foreach (Type t in reportTypes)
            {
                IReport<T> report = (IReport<T>)Activator.CreateInstance(t);
                result.Add(report);
            }
            return result;
        }

        public static IReport<T> RetrieveReport<T>(string key) where T : IReportCriteria
        {
            return RetrieveReports<T>().FirstOrDefault(x => x.Key == key);
        }

        public static IReport<T> RetrieveReport<T>(string key, T criteria) where T : IReportCriteria
        {
            return RetrieveReport<T>(key).Configure(criteria);
        }

        public static IReport RetrieveReport(string key)
        {
            return RetrieveReports().FirstOrDefault(x => x.Key == key);
        }

        public static IList<ReportCategory<T>> RetrieveCategories<T>() where T : IReportCriteria
        {
            List<ReportCategory<T>> result = new List<ReportCategory<T>>();
            IList<IReport<T>> reports = RetrieveReports<T>();
            foreach (IReport<T> r in reports)
            {
                ReportCategory<T> rc = result.FirstOrDefault(x => x.Name == r.CategoryName);
                if (rc == null)
                {
                    rc = new ReportCategory<T>(r.CategoryName);
                    result.Add(rc);
                }
                rc.AddReport(r);
            }
            return result;
        }

        public static string Script()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<dynamic> resources = new List<dynamic>();
            foreach (Assembly a in assemblies)
            {
                if (!a.IsDynamic)
                {
                    string[] names = a.GetManifestResourceNames();
                    string n = names.Where(x => x.Contains("_reports.js")).FirstOrDefault();
                    if (!string.IsNullOrEmpty(n))
                        resources.Add(new { assembly = a, name = n });
                }
            }

            if (resources.Count == 0) return string.Empty;

            string result = string.Empty;

            foreach (var r in resources)
            {
                string resourceName = r.name;
                Assembly assembly = r.assembly;

                if (string.IsNullOrEmpty(resourceName))
                    throw new ArgumentNullException("resourceName");

                if (assembly == null)
                    throw new ArgumentNullException("assembly");

                using (System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    result += reader.ReadToEnd();
                    result += Environment.NewLine + Environment.NewLine;
                    reader.Close();
                }
            }

            return result;
        }
    }
}
