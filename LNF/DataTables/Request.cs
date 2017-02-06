using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace LNF.DataTables
{
    public class Request
    {
        public Request() { }

        public static Request Create(NameValueCollection data)
        {
            if (data == null)
                return null;

            Request result = new Request();
            result.Draw = int.Parse(data["draw"]);
            result.Start = int.Parse(data["start"]);
            result.Length = int.Parse(data["length"]);
            result.GetSearch(data);
            result.GetOrder(data);
            result.GetColumns(data);
            return result;
        }

        public static Request Create(IDictionary<string, string> data)
        {
            if (data == null)
                return null;

            Request result = new Request();
            result.Draw = int.Parse(data.First(kvp => kvp.Key == "draw").Value);
            result.Start = int.Parse(data.First(kvp => kvp.Key == "start").Value);
            result.Length = int.Parse(data.First(kvp => kvp.Key == "length").Value);
            result.GetSearch(data);
            result.GetOrder(data);
            result.GetColumns(data);
            return result;
        }

        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public Search Search { get; set; }
        public List<Order> Order { get; set; }
        public List<Column> Columns { get; set; }

        private void GetColumns(NameValueCollection data)
        {
            Columns = new List<Column>();

            int index = 0;
            while (true)
            {
                string key = string.Format("columns[{0}]", index);
                string[] items = data.AllKeys.Where(x => x.StartsWith(key)).ToArray();
                if (items.Length > 0)
                {
                    Column c = new Column();
                    c.Data = data[items.First(x => x == key + "[data]")];
                    c.Name = data[items.First(x => x == key + "[name]")];
                    c.Orderable = bool.Parse(data[items.First(x => x == key + "[orderable]")]);
                    c.Searchable = bool.Parse(data[items.First(x => x == key + "[searchable]")]);
                    c.Search = new ColumnSearch();
                    c.Search.Regex = bool.Parse(data[items.First(x => x == key + "[search][regex]")]);
                    c.Search.Value = data[items.First(x => x == key + "[search][value]")];
                    Columns.Add(c);
                    index++;
                }
                else
                    break;
            }
        }

        private void GetColumns(IDictionary<string, string> data)
        {
            Columns = new List<Column>();

            int index = 0;
            while (true)
            {
                string key = string.Format("columns[{0}]", index);
                KeyValuePair<string, string>[] items = data.Where(x => x.Key.StartsWith(key)).ToArray();
                if (items.Length > 0)
                {
                    Column c = new Column();
                    c.Data = items.First(kvp => kvp.Key == key + "[data]").Value;
                    c.Name = items.First(kvp => kvp.Key == key + "[name]").Value;
                    c.Orderable = bool.Parse(items.First(kvp => kvp.Key == key + "[orderable]").Value);
                    c.Searchable = bool.Parse(items.First(kvp => kvp.Key == key + "[searchable]").Value);
                    c.Search = new ColumnSearch();
                    c.Search.Regex = bool.Parse(items.First(kvp => kvp.Key == key + "[search][regex]").Value);
                    c.Search.Value = items.First(kvp => kvp.Key == key + "[search][value]").Value;
                    Columns.Add(c);
                    index++;
                }
                else
                    break;
            }
        }

        private void GetOrder(NameValueCollection data)
        {
            Order = new List<Order>();

            int index = 0;
            while (true)
            {
                string key = string.Format("order[{0}]", index);
                string[] items = data.AllKeys.Where(x => x.StartsWith(key)).ToArray();
                if (items.Length > 0)
                {
                    Order o = new Order();
                    o.Column = int.Parse(data[items.First(x => x == key + "[column]")]);
                    o.Dir = data[items.First(x => x == key + "[dir]")];
                    Order.Add(o);
                    index++;
                }
                else
                    break;
            }
        }

        private void GetOrder(IDictionary<string, string> data)
        {
            Order = new List<Order>();

            int index = 0;
            while (true)
            {
                string key = string.Format("order[{0}]", index);
                KeyValuePair<string, string>[] items = data.Where(x => x.Key.StartsWith(key)).ToArray();
                if (items.Length > 0)
                {
                    Order o = new Order();
                    o.Column = int.Parse(items.First(kvp => kvp.Key == key + "[column]").Value);
                    o.Dir = items.First(kvp => kvp.Key == key + "[dir]").Value;
                    Order.Add(o);
                    index++;
                }
                else
                    break;
            }
        }

        private void GetSearch(NameValueCollection data)
        {
            Search = new Search();

            string key = "search";
            string[] items = data.AllKeys.Where(x => x.StartsWith(key)).ToArray();
            if (items.Length > 0)
            {
                Search.Regex = bool.Parse(data[items.First(x => x == key + "[regex]")]);
                Search.Value = data[items.First(x => x == key + "[value]")];
            }
        }

        private void GetSearch(IDictionary<string, string> data)
        {
            Search = new Search();

            string key = "search";
            KeyValuePair<string, string>[] items = data.Where(x => x.Key.StartsWith(key)).ToArray();
            if (items.Length > 0)
            {
                Search.Regex = bool.Parse(items.First(kvp => kvp.Key == key + "[regex]").Value);
                Search.Value = items.First(kvp => kvp.Key == key + "[value]").Value;
            }
        }
    }
}
