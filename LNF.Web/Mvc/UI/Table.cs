using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;

namespace LNF.Web.Mvc.UI
{
    public class Table<T>
    {
        private AttributeCollection<Table<T>> _Attributes;
        private Action<TableHeaderRow<T>> _HeaderRowFormatter;
        private Action<TableItemRow<T>> _ItemRowFormatter;
        private Action<TableNoDataRow> _NoDataRowFormatter;
        private Action<TableFooterRow> _FooterRowFormatter;

        public IEnumerable<T> DataSource { get; set; }
        public AttributeCollection<Table<T>> Attributes { get { return _Attributes; } }
        public Action<TableHeaderRow<T>> HeaderRowFormatter { get { return _HeaderRowFormatter; } }
        public Action<TableItemRow<T>> ItemRowFormatter { get { return _ItemRowFormatter; } }
        public Action<TableNoDataRow> NoDataRowFormatter { get { return _NoDataRowFormatter; } }
        public Action<TableFooterRow> FooterRowFormater { get { return _FooterRowFormatter; } }

        public TimeSpan BuildTime { get; set; }

        public Table(IEnumerable<T> dataSource = null)
        {
            DataSource = dataSource;
            _Attributes = new AttributeCollection<Table<T>>(this);
        }

        public Table<T> SetHeaderRowFormatter(Action<TableHeaderRow<T>> action)
        {
            _HeaderRowFormatter = action;
            return this;
        }

        public Table<T> SetItemRowFormatter(Action<TableItemRow<T>> action)
        {
            _ItemRowFormatter = action;
            return this;
        }

        public Table<T> SetNoDataRowFormatter(Action<TableNoDataRow> action)
        {
            _NoDataRowFormatter = action;
            return this;
        }

        public Table<T> SetFooterRowFormatter(Action<TableFooterRow> action)
        {
            _FooterRowFormatter = action;
            return this;
        }

        public IHtmlString GetHtml(IEnumerable<T> dataSource, IEnumerable<TableColumn<T>> columns)
        {
            DataSource = dataSource;
            return GetHtml(columns);
        }

        public IHtmlString GetHtml(IEnumerable<TableColumn<T>> columns)
        {
            DateTime startTime = DateTime.Now;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("<table{0}>", _Attributes));
            TableHeaderRow<T> header;
            int index = 0;
            header = new TableHeaderRow<T>(index, columns);

            if (_HeaderRowFormatter != null)
                _HeaderRowFormatter(header);

            if (header.Visible)
            {
                sb.AppendLine("<thead>");
                sb.AppendLine(string.Format("<tr{0}>", header.Attributes));
                foreach (TableColumn<T> col in columns)
                {
                    sb.AppendLine(string.Format("<th{0}>{1}</th>", col.HeaderCellAttributes, (string.IsNullOrEmpty(col.HeaderText)) ? "&nbsp;" : col.HeaderText));
                }
                sb.AppendLine("</tr>");
                sb.AppendLine("</thead>");
                index++;
            }
            
            if (DataSource != null && DataSource.Count() > 0)
            {
                sb.AppendLine("<tbody>");
                foreach (T obj in DataSource)
                {
                    TableItemRow<T> row = new TableItemRow<T>(index, obj);
                    if (_ItemRowFormatter != null)
                        _ItemRowFormatter(row);
                    if (row.Visible)
                    {
                        if (index == 1 && row.FirstRowAttributes.Count > 0)
                            sb.AppendLine(string.Format("<tr{0}>", row.FirstRowAttributes));
                        else if ((index % 2) == 0 && row.EvenRowAttributes.Count > 0)
                            sb.AppendLine(string.Format("<tr{0}>", row.EvenRowAttributes));
                        else if ((index % 2) != 0 && row.OddRowAttributes.Count > 0)
                            sb.AppendLine(string.Format("<tr{0}>", row.OddRowAttributes));
                        else if (index == DataSource.Count() && row.LastRowAttributes.Count > 0)
                            sb.AppendLine(string.Format("<tr{0}>", row.LastRowAttributes));
                        else
                            sb.AppendLine(string.Format("<tr{0}>", row.Attributes));
                        foreach (TableColumn<T> col in columns)
                        {
                            string val = col.GetItemCellValue(obj);
                            sb.AppendLine(string.Format("<td{0}>{1}</td>", col.ItemCellAttributes, (string.IsNullOrEmpty(val)) ? "&nbsp;" : val.ToString()));
                        }
                        sb.AppendLine("</tr>");
                        index++;
                    }
                }
                sb.AppendLine("</tbody>");
            }
            else
            {
                TableNoDataRow row = new TableNoDataRow(index, string.Empty);
                if (_NoDataRowFormatter != null)
                {
                    _NoDataRowFormatter(row);
                    if (!string.IsNullOrEmpty(row.Message) && row.Visible)
                    {
                        sb.AppendLine("<tbody>");
                        sb.AppendLine(string.Format("<tr{0}>", row.Attributes));
                        sb.AppendLine(string.Format("<td{0} colspan=\"{1}\">{2}</td>", row.CellAttributes, columns.Count(), row.Message));
                        sb.AppendLine("</tr>");
                        sb.AppendLine("</tbody>");
                        index++;
                    }
                }
            }

            

            TableFooterRow footerRow = new TableFooterRow(index, string.Empty);
            if (_FooterRowFormatter != null)
            {
                _FooterRowFormatter(footerRow);
                if (footerRow.Visible)
                {
                    sb.AppendLine("<tfoot>");
                    sb.AppendLine(string.Format("<tr{0}>", footerRow.Attributes));
                    sb.AppendLine(footerRow.Html);
                    sb.AppendLine("</tr>");
                    sb.AppendLine("</tfoot>");
                    index++;
                }
            }

            sb.AppendLine("</table>");

            BuildTime = DateTime.Now - startTime;

            return new HtmlString(sb.ToString());
        }

        public TableColumn<T>[] Columns(params TableColumn<T>[] columnSet)
        {
            return columnSet;
        }

        public TableColumn<T> Column(string headerText = null)
        {
            TableColumn<T> result = new TableColumn<T>();
            result.HeaderText = headerText;
            return result;
        }
    }

    public class TableColumn<T>
    {
        private Func<T, string> _ItemCellFormatter = null;
        private AttributeCollection<TableColumn<T>> _HeaderCellAttributes;
        private AttributeCollection<TableColumn<T>> _ItemCellAttributes;

        public string HeaderText { get; set; }
        public Func<T, string> ItemCellFormatter { get { return _ItemCellFormatter; } }
        public AttributeCollection<TableColumn<T>> HeaderCellAttributes { get { return _HeaderCellAttributes; } }
        public AttributeCollection<TableColumn<T>> ItemCellAttributes { get { return _ItemCellAttributes; } }

        internal TableColumn()
        {
            _ItemCellAttributes = new AttributeCollection<TableColumn<T>>(this);
            _HeaderCellAttributes = new AttributeCollection<TableColumn<T>>(this);
        }

        public static TableColumn<T> Create(Func<T, string> itemCellFormatter, string headerText = null, object itemCellAttributes = null)
        {
            return new TableColumn<T>()
                .SetHeaderText(headerText)
                .SetItemCellFormatter(itemCellFormatter)
                .ItemCellAttributes.Add(itemCellAttributes);
        }

        public TableColumn<T> SetHeaderText(string text)
        {
            HeaderText = text;
            return this;
        }

        public TableColumn<T> SetItemCellFormatter(Func<T, string> function)
        {
            _ItemCellFormatter = function;
            return this;
        }

        public string GetItemCellValue(T obj)
        {
            string result = null;

            if (_ItemCellFormatter != null)
                result = _ItemCellFormatter(obj);

            return result;
        }
    }

    public class TableItemRow<T>
    {
        internal TableItemRow(int rowIndex, T data)
        {
            _RowIndex = rowIndex;
            _Data = data;
            Visible = true;
            _Attributes = new AttributeCollection<TableItemRow<T>>(this);
            _EvenRowAttributes = new AttributeCollection<TableItemRow<T>>(this);
            _OddRowAttributes = new AttributeCollection<TableItemRow<T>>(this);
            _FirstRowAttributes = new AttributeCollection<TableItemRow<T>>(this);
            _LastRowAttributes = new AttributeCollection<TableItemRow<T>>(this);
        }

        private int _RowIndex;
        private T _Data;
        private AttributeCollection<TableItemRow<T>> _Attributes;
        private AttributeCollection<TableItemRow<T>> _EvenRowAttributes;
        private AttributeCollection<TableItemRow<T>> _OddRowAttributes;
        private AttributeCollection<TableItemRow<T>> _FirstRowAttributes;
        private AttributeCollection<TableItemRow<T>> _LastRowAttributes;

        public int RowIndex { get { return _RowIndex; } }
        public T Data { get { return _Data; } }
        public bool Visible { get; set; }
        public AttributeCollection<TableItemRow<T>> Attributes { get { return _Attributes; } }
        public AttributeCollection<TableItemRow<T>> EvenRowAttributes { get { return _EvenRowAttributes; } }
        public AttributeCollection<TableItemRow<T>> OddRowAttributes { get { return _OddRowAttributes; } }
        public AttributeCollection<TableItemRow<T>> FirstRowAttributes { get { return _FirstRowAttributes; } }
        public AttributeCollection<TableItemRow<T>> LastRowAttributes { get { return _LastRowAttributes; } }
    }

    public class TableHeaderRow<T>
    {
        private int _RowIndex;
        private IEnumerable<TableColumn<T>> _Columns;
        private AttributeCollection<TableHeaderRow<T>> _Attributes;

        public int RowIndex { get { return _RowIndex; } }
        public IEnumerable<TableColumn<T>> Columns { get { return _Columns; } }
        public AttributeCollection<TableHeaderRow<T>> Attributes { get { return _Attributes; } }
        public bool Visible { get; set; }

        internal TableHeaderRow(int rowIndex, IEnumerable<TableColumn<T>> columns)
        {
            _RowIndex = rowIndex;
            _Columns = columns;
            Visible = true;
            _Attributes = new AttributeCollection<TableHeaderRow<T>>(this);
        }
    }

    public class TableNoDataRow
    {
        private int _RowIndex;
        private AttributeCollection<TableNoDataRow> _Attributes;
        private AttributeCollection<TableNoDataRow> _CellAttributes;

        public int RowIndex { get; set; }
        public string Message { get; set; }
        public bool Visible { get; set; }
        public AttributeCollection<TableNoDataRow> Attributes { get { return _Attributes; } }
        public AttributeCollection<TableNoDataRow> CellAttributes { get { return _CellAttributes; } }

        internal TableNoDataRow(int rowIndex, string message)
        {
            _RowIndex = rowIndex;
            Message = message;
            Visible = true;
            _Attributes = new AttributeCollection<TableNoDataRow>(this);
            _CellAttributes = new AttributeCollection<TableNoDataRow>(this);
        }
    }

    public class TableFooterRow
    {
        private int _RowIndex;
        private string _Html;
        private AttributeCollection<TableFooterRow> _Attributes;

        public int RowIndex { get; set; }
        public bool Visible { get; set; }
        public AttributeCollection<TableFooterRow> Attributes { get { return _Attributes; } }
        public string Html { get { return _Html; } }

        internal TableFooterRow(int rowIndex, string html)
        {
            _RowIndex = rowIndex;
            Visible = true;
            _Attributes = new AttributeCollection<TableFooterRow>(this);
        }

        public TableFooterRow SetHtml(string value)
        {
            _Html = value;
            return this;
        }

        public TableFooterRow AddColumn(string innerHtml, AttributeCollection<TableFooterRow> attribs = null)
        {
            if (attribs != null)
                _Html += string.Format("<td{0}>", attribs);
            else
                _Html += "<td>";
            _Html += innerHtml;
            _Html += "</td>";
            return this;
        }

        public AttributeCollection<TableFooterRow> CreateColumnAttributes(string key, string value)
        {
            AttributeCollection<TableFooterRow> result = new AttributeCollection<TableFooterRow>(this);
            result.Add(key, value);
            return result;
        }

        public AttributeCollection<TableFooterRow> CreateColumnAttributes(params KeyValuePair<string, string>[] attributes)
        {
            AttributeCollection<TableFooterRow> result = new AttributeCollection<TableFooterRow>(this);
            result.Add(attributes);
            return result;
        }
    }

    public static class Attr
    {
        public static KeyValuePair<string, string> Create(string key, string value)
        {
            return new KeyValuePair<string, string>(key, value);
        }
    }

    public class AttributeCollection<T> : IEnumerable<KeyValuePair<string, string>>
    {
        private T owner;
        private Dictionary<string, string> items;

        internal AttributeCollection(T owner)
        {
            this.owner = owner;
            items = new Dictionary<string, string>();
        }

        public T Add(string key, string value)
        {
            if (items.ContainsKey(key))
                items[key] = value;
            else
                items.Add(key, value);

            return owner;
        }

        public T Add(params KeyValuePair<string, string>[] attributes)
        {
            foreach (KeyValuePair<string, string> kvp in attributes)
            {
                Add(kvp.Key, kvp.Value);
            }
            return owner;
        }

        public T Add(object htmlAttributes)
        {
            if (htmlAttributes == null) return owner;
            return Add(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public T Add(IDictionary<string, object> htmlAttributes)
        {
            foreach (KeyValuePair<string, object> kvp in htmlAttributes)
            {
                Add(kvp.Key, kvp.Value.ToString());
            }
            return owner;
        }

        public T Remove(string key)
        {
            if (items.ContainsKey(key))
                items.Remove(key);
            return owner;
        }

        public T Clear()
        {
            items.Clear();
            return owner;
        }

        public int Count
        {
            get { return items.Count; }
        }

        public override string ToString()
        {
            string result = string.Empty;
            foreach (KeyValuePair<string, string> kvp in items)
            {
                result += string.Format(" {0}=\"{1}\"", kvp.Key, kvp.Value.Replace("\"", "\\\""));
            }
            return result;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public static class TableUtility
    {
        public static void SetOrAddAttribute(Dictionary<string, string> attr, string key, string value)
        {
            if (attr.ContainsKey(key))
                attr[key] = value;
            else
                attr.Add(key, value);
        }

        public static string AttributesToString(Dictionary<string, string> attr)
        {
            string result = string.Empty;
            foreach (KeyValuePair<string, string> kvp in attr)
            {
                result += string.Format(" {0}=\"{1}\"", kvp.Key, kvp.Value.Replace("\"", "\\\""));
            }
            return result;
        }
    }
}
