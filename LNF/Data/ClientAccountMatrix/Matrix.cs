using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Data.ClientAccountMatrix
{
    public class Matrix : IEnumerable<KeyValuePair<int, MatrixItem>>
    {
        private IEnumerable<ClientAccountAssignment> source;
        private Dictionary<int, MatrixItem> _Items;
        private List<MatrixUserHeader> _Headers;
        private StringBuilder _FilterHtml;
        private StringBuilder _MatrixHtml;
        public int GroupSize { get; set; }
        public bool ReadOnly { get; set; }

        public Matrix(int managerOrgId, bool readOnly = false, int groupSize = 10)
        {
            source = DA.Current.Query<ClientAccountAssignment>().Where(x => x.ManagerOrgID == managerOrgId).ToList()
                //.Where(x => x.Display() && PrivUtility.HasPriv(x.EmployeePrivs, Matrix.PrivFilter))
                .Where(IncludeOnMatrix)
                .OrderBy(x => x.AccountName)
                .OrderBy(x => x.GetEmployeeDisplayName())
                .ToList();

            ReadOnly = readOnly;
            GroupSize = groupSize;

            RefreshHeaderAndFilter();
            RefreshMatrix();
        }

        public bool IncludeOnMatrix(ClientAccountAssignment x)
        {
            bool res = x.Display() && PrivUtility.HasPriv(x.EmployeePrivs, PrivFilter);
            return res;
        }

        public Matrix(IEnumerable<ClientAccountAssignment> dataSource, bool readOnly = false, int groupSize = 10)
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
                string value = ServiceProvider.Current.Context.GetAppSetting("ClientAccountMatrix.PrivFilter");
                if (!string.IsNullOrEmpty(value))
                    return PrivUtility.CalculatePriv(value.Split(','));
                return defval;
            }
        }

        public void RefreshHeaderAndFilter()
        {
            _Headers = new List<MatrixUserHeader>();

            _FilterHtml = new StringBuilder();
            _FilterHtml.AppendLine("<select>");

            //Use the first item's account to get each user name. Each account
            //should have the same list of users becuase of the way the view works.
            ClientAccountAssignment first = source.FirstOrDefault();
            if (first != null)
            {
                int index = 0;
                int groupNumber = 0;
                string key = string.Empty;
                List<ClientAccountAssignment> items = source.Where(x => x.AccountID == first.AccountID).ToList();
                foreach (ClientAccountAssignment caa in items)
                {
                    if (index % GroupSize == 0)
                    {
                        key = "opt-group-" + groupNumber.ToString();
                        string lname1 = caa.EmployeeLastName;
                        string lname2 = items[Math.Min(index + (GroupSize - 1), items.Count - 1)].EmployeeLastName;
                        string text = string.Format("{0} - {1}", lname1, lname2);
                        _FilterHtml.AppendFormat("<option value=\"{0}\">{1}</option>", key, text);
                        groupNumber++;
                    }

                    MatrixUserHeader uh = new MatrixUserHeader
                    {
                        ClientOrgID = caa.ClientOrgID,
                        DisplayName = caa.GetEmployeeDisplayName(),
                        Key = key
                    };

                    _Headers.Add(uh);

                    index++;
                }
            }

            _FilterHtml.AppendLine("</select>");
        }

        public void RefreshMatrix()
        {
            _Items = new Dictionary<int, MatrixItem>();

            foreach (ClientAccountAssignment caa in source)
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
                        Project = caa.GetAccountProject(),
                        ShortCode = caa.ShortCode
                    }, ReadOnly);

                    _Items.Add(acctID, item);
                }

                item = this[acctID];

                MatrixUserAccount ua = new MatrixUserAccount
                {
                    AccountID = acctID,
                    ClientOrgID = caa.ClientOrgID,
                    DisplayName = caa.GetEmployeeDisplayName(),
                    Email = caa.EmployeeEmail
                };

                if (caa.IsAssigned())
                {
                    ua.ClientAccountID = caa.EmployeeClientAccountID;
                    ua.WarningMessage = caa.HasDryBox ? "Client has a drybox reserved with this account." : string.Empty;
                    ua.Active = caa.EmployeeClientAccountActive;
                }

                MatrixUserHeader uh = _Headers.Where(x => x.ClientOrgID == ua.ClientOrgID).First();
                ua.Key = uh.Key;

                item[ua.ClientOrgID] = ua;
            }

            _MatrixHtml = new StringBuilder();
            string cssAttribute = " class=\"matrix\"";
            string startTag = string.Format("<table{0}>", cssAttribute);
            _MatrixHtml.AppendLine(startTag);

            _MatrixHtml.AppendLine(HeaderRowHtml.ToString());

            foreach (KeyValuePair<int, MatrixItem> kvp in _Items)
            {
                _MatrixHtml.AppendLine(kvp.Value.MatrixItemHtml.ToString());
            }

            _MatrixHtml.AppendLine("</table>");
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

        public StringBuilder FilterHtml
        {
            get { return _FilterHtml; }
        }

        public StringBuilder MatrixHtml
        {
            get { return _MatrixHtml; }
        }

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