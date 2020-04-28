using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LNF.Data.ClientAccountMatrix
{
    public class MatrixItem : IEnumerable<MatrixUserAccount>
    {
        private StringBuilder _MatrixItemHtml;
        private List<MatrixUserAccount> _Users;
        private int colIndex;
        private readonly bool readOnly;

        public MatrixAccount Account { get; }

        public StringBuilder MatrixItemHtml
        {
            get
            {
                StringBuilder result = new StringBuilder(_MatrixItemHtml.ToString());
                result.AppendLine("</tr>");
                return result;
            }
        }

        public MatrixItem(MatrixAccount account, bool readOnly)
        {
            Account = account;
            _Users = new List<MatrixUserAccount>();
            _MatrixItemHtml = new StringBuilder();
            _MatrixItemHtml.AppendFormat("<tr data-account-id=\"{0}\">", Account.AccountID);
            _MatrixItemHtml.Append("<th style=\"text-align: left; white-space: nowrap;\">");
            _MatrixItemHtml.AppendFormat("<div class=\"acct-name\" style=\"display: block;\">{0}</div><div class=\"acct-number\" style=\"display: none;\">{1}</div>", Account.Name, Account.Number);
            _MatrixItemHtml.AppendFormat("<div class=\"acct-project\" style=\"display: none;\">{0}</div><div class=\"acct-shortcode\" style=\"display: none;\">{1}</div>", Account.Project, (string.IsNullOrEmpty(Account.ShortCode.Trim())) ? "<span class=\"nodata\">[none]</span>" : Account.ShortCode);
            _MatrixItemHtml.Append("</th>");
            colIndex = 1;
            this.readOnly = readOnly;
        }

        public MatrixUserAccount this[int ClientOrgID]
        {
            get
            {
                return _Users.Find(delegate(MatrixUserAccount ua) { return ua.ClientOrgID == ClientOrgID; });
            }
            set
            {
                //if there is already an item for this ClientOrgID then silently ignore
                if (!Contains(ClientOrgID))
                {
                    _Users.Add(value);

                    string colClass = "col-" + colIndex;
                    string checkedAttribute = (value.ClientAccountID == 0 || !value.Active) ? string.Empty : " checked=\"checked\"";
                    _MatrixItemHtml.AppendFormat("<td class=\"user-column {0} {1}\" style=\"text-align: center;\">", value.Key, colClass);
                    if (readOnly)
                    {
                        _MatrixItemHtml.Append(string.IsNullOrEmpty(checkedAttribute) ? string.Empty : "&#10004;");
                    }
                    else
                    {
                        _MatrixItemHtml.AppendFormat("<input type=\"checkbox\"{0} class=\"user-account-checkbox\" data-client-org-id=\"{1}\" data-display-name=\"{2}\" data-email=\"{3}\" data-warning-message=\"{4}\" />",
                            checkedAttribute,
                            value.ClientOrgID,
                            value.DisplayName,
                            value.Email,
                            value.WarningMessage);
                    }
                    _MatrixItemHtml.Append("</td>");

                    colIndex++;
                }
            }
        }

        public bool Contains(int ClientOrgID)
        {
            return this[ClientOrgID] != null;
        }

        //public void Render(StringBuilder sb, List<MatrixUserHeader> headers)
        //{

        //    foreach (MatrixUserAccount ua in Users.OrderBy(i => i.DisplayName))
        //    {
        //        MatrixUserHeader uh = headers.FirstOrDefault(i => i.ClientOrgID == ua.ClientOrgID);
        //        string col_class = "col-" + col_index;
        //        if (uh != null)
        //        {
        //            string checked_attribute = (ua.ClientAccountID == 0 || !ua.Active) ? string.Empty : " checked=\"checked\"";
        //            sb.AppendFormat("<td class=\"user-column {0} {1}\" style=\"text-align: center;\">", uh.Key, col_class);
        //            sb.AppendFormat("<input type=\"checkbox\"{0} class=\"user-account-checkbox\" data-client-org-id=\"{1}\" data-display-name=\"{2}\" data-warning-message=\"{3}\" />",
        //                checked_attribute,
        //                ua.ClientOrgID,
        //                ua.DisplayName,
        //                ua.WarningMessage);
        //            sb.Append("</td>");
        //        }
        //        col_index += 1;
        //    }
        //    sb.Append("</tr>");
        //}

        public IEnumerator<MatrixUserAccount> GetEnumerator()
        {
            return _Users.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}