using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace LNF.Reporting
{
    public class DefaultCriteria : IReportCriteria
    {
        private Dictionary<string, string> parameters;

        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DefaultCriteria()
        {
            parameters = new Dictionary<string, string>();
        }

        public DefaultCriteria(params NameValueCollection[] nvc)
            : this()
        {
            foreach (NameValueCollection item in nvc)
            {
                foreach (string key in item.AllKeys)
                {
                    if (!parameters.ContainsKey(key))
                        parameters.Add(key, item[key]);
                    else
                        parameters[key] = item[key];
                }
            }
        }

        public T GetValue<T>(string key, T defval)
        {
            if (!parameters.ContainsKey(key))
                return defval;
            string raw = parameters[key];
            if (string.IsNullOrEmpty(raw)) return defval;
            return RepositoryUtility.ConvertTo(raw, defval);
        }

        public IEnumerable<Client> ActiveClients()
        {
            return ActiveLogUtility.FindActive<Client>(x => x.ClientID, StartDate, EndDate);
        }

        public CriteriaWriter CreateWriter(StringBuilder sb)
        {
            return new CriteriaWriter(sb);
        }
    }
}
