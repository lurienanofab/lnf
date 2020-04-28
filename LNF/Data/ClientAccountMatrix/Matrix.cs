using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LNF.Data.ClientAccountMatrix
{
    public class Matrix : IEnumerable<KeyValuePair<int, MatrixItem>>
    {
        private IEnumerable<IClientAccountAssignment> source;
        private Dictionary<int, MatrixItem> _Items;
        private List<MatrixUserHeader> _Headers;

        public int GroupSize { get; set; }
        public bool ReadOnly { get; set; }

        public Matrix(int managerOrgId, bool readOnly = false, int groupSize = 10)
        {
            source = ServiceProvider.Current.Data.Client.GetClientAccountAssignments(managerOrgId)
                .Where(IncludeOnMatrix)
                .OrderBy(x => x.AccountName)
                .OrderBy(x => GetEmployeeDisplayName(x))
                .ToList();

            ReadOnly = readOnly;
            GroupSize = groupSize;

            RefreshHeaderAndFilter();
            RefreshMatrix();
        }

        public bool IncludeOnMatrix(IClientAccountAssignment x)
        {
            bool res = Display(x) && PrivUtility.HasPriv(x.EmployeePrivs, PrivFilter);
            return res;
        }

        public Matrix(IEnumerable<IClientAccountAssignment> dataSource, bool readOnly = false, int groupSize = 10)
        {
            source = dataSource;
            ReadOnly = readOnly;
            GroupSize = groupSize;

            RefreshHeaderAndFilter();
            RefreshMatrix();
        }

        public static ClientPrivilege PrivFilter
        {
            get
            {
                ClientPrivilege defval = ClientPrivilege.LabUser | ClientPrivilege.Staff | ClientPrivilege.StoreUser | ClientPrivilege.RemoteUser;
                string value = ConfigurationManager.AppSettings["ClientAccountMatrix.PrivFilter"];
                if (!string.IsNullOrEmpty(value))
                    return PrivUtility.CalculatePriv(value.Split(','));
                return defval;
            }
        }

        public void RefreshHeaderAndFilter()
        {
            _Headers = new List<MatrixUserHeader>();

            FilterHtml = new StringBuilder();
            FilterHtml.AppendLine("<select>");

            //Use the first item's account to get each user name. Each account
            //should have the same list of users becuase of the way the view works.
            IClientAccountAssignment first = source.FirstOrDefault();
            if (first != null)
            {
                int index = 0;
                int groupNumber = 0;
                string key = string.Empty;
                List<IClientAccountAssignment> items = source.Where(x => x.AccountID == first.AccountID).ToList();
                foreach (IClientAccountAssignment caa in items)
                {
                    if (index % GroupSize == 0)
                    {
                        key = "opt-group-" + groupNumber.ToString();
                        string lname1 = caa.EmployeeLastName;
                        string lname2 = items[Math.Min(index + (GroupSize - 1), items.Count - 1)].EmployeeLastName;
                        string text = string.Format("{0} - {1}", lname1, lname2);
                        FilterHtml.AppendFormat("<option value=\"{0}\">{1}</option>", key, text);
                        groupNumber++;
                    }

                    MatrixUserHeader uh = new MatrixUserHeader
                    {
                        ClientOrgID = caa.ClientOrgID,
                        DisplayName = GetEmployeeDisplayName(caa),
                        Key = key
                    };

                    _Headers.Add(uh);

                    index++;
                }
            }

            FilterHtml.AppendLine("</select>");
        }

        public void RefreshMatrix()
        {
            _Items = new Dictionary<int, MatrixItem>();

            foreach (IClientAccountAssignment caa in source)
            {
                int acctID = caa.AccountID;
                MatrixItem item;
                if (!ContainsKey(acctID))
                {
                    item = new MatrixItem(new MatrixAccount
                    {
                        AccountID = acctID,
                        Name = caa.AccountName,
                        Number = caa.AccountNumber,
                        Project = GetAccountProject(caa),
                        ShortCode = caa.ShortCode
                    }, ReadOnly);

                    _Items.Add(acctID, item);
                }

                item = this[acctID];

                MatrixUserAccount ua = new MatrixUserAccount
                {
                    AccountID = acctID,
                    ClientOrgID = caa.ClientOrgID,
                    DisplayName = GetEmployeeDisplayName(caa),
                    Email = caa.EmployeeEmail
                };

                if (IsAssigned(caa))
                {
                    ua.ClientAccountID = caa.EmployeeClientAccountID;
                    ua.WarningMessage = caa.HasDryBox ? "Client has a drybox reserved with this account." : string.Empty;
                    ua.Active = caa.EmployeeClientAccountActive;
                }

                MatrixUserHeader uh = _Headers.Where(x => x.ClientOrgID == ua.ClientOrgID).First();
                ua.Key = uh.Key;

                item[ua.ClientOrgID] = ua;
            }

            MatrixHtml = new StringBuilder();
            string cssAttribute = " class=\"matrix\"";
            string startTag = string.Format("<table{0}>", cssAttribute);
            MatrixHtml.AppendLine(startTag);

            MatrixHtml.AppendLine(HeaderRowHtml.ToString());

            foreach (KeyValuePair<int, MatrixItem> kvp in _Items)
            {
                MatrixHtml.AppendLine(kvp.Value.MatrixItemHtml.ToString());
            }

            MatrixHtml.AppendLine("</table>");
        }

        public MatrixItem this[int key]
        {
            get { return _Items[key]; }
        }

        public int AccountCount
        {
            get { return _Items.Count; }
        }

        public int EmployeeCount
        {
            get { return _Headers.Count; }
        }

        public bool ContainsKey(int key)
        {
            return _Items.ContainsKey(key);
        }

        public StringBuilder FilterHtml { get; private set; }

        public StringBuilder MatrixHtml { get; private set; }

        public StringBuilder HeaderRowHtml
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (_Items.Count > 0)
                {
                    sb.Append("<tr>");
                    sb.Append("<th>&nbsp;</th>");
                    foreach (MatrixUserHeader uh in _Headers)
                    {
                        uh.Render(sb);
                    }
                    sb.Append("</tr>");
                }
                return sb;
            }
        }

        private string GetEmployeeDisplayName(IClientAccountAssignment x) => Clients.GetDisplayName(x.EmployeeLastName, x.EmployeeFirstName);

        private string GetManagerDisplayName(IClientAccountAssignment x) => Clients.GetDisplayName(x.ManagerLastName, x.ManagerFirstName);

        private bool Display(IClientAccountAssignment x)
        {
            return x.ClientManagerActive
                && x.ManagerClientActive
                && x.ManagerClientOrgActive
                && x.ManagerClientAccountActive
                && x.EmployeeClientActive
                && x.EmployeeClientOrgActive
                && x.AccountActive
                && x.Manager;
        }

        private bool IsAssigned(IClientAccountAssignment x)
        {
            return x.EmployeeClientAccountID > 0;
        }

        private string GetAccountProject(IClientAccountAssignment x) => Accounts.GetProject(x.AccountNumber);

        public IEnumerator<KeyValuePair<int, MatrixItem>> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_Items as IEnumerable).GetEnumerator();
        }
    }
}