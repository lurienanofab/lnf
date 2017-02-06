using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.DataTables
{
    public class DataTablesResult<T>
    {
        //private int iColumns;
        //private string sColumns;
        //private int iDisplayStart;
        //private int iDisplayLength;
        //private string sSearchGlobal;
        //private bool bRegexGlobal;
        //private int iSortingCols;

        //private List<string> mDataProp;
        //private List<string> sSearch;
        //private List<bool> bRegex;
        //private List<bool> bSearchable;
        //private List<int> iSortCol;
        //private List<string> sSortDir;
        //private List<bool> bSortable;

        private Request _Request;
        private Response _Response;

        private IList<T> _Query = null;
        private IList<T> _Page = null;
        private List<Property<T>> _Properties;
        private Func<IList<T>, IOrderedEnumerable<T>> _DefaultSort;

        public Request Request { get { return _Request; } }
        public Response Response { get { return _Response; } }

        /// <summary>
        /// Represents a DataTables server response
        /// </summary>
        /// <param name="data">The request data sent by the client</param>
        public DataTablesResult(IDictionary<string, string> data)
        {
            _Request = Request.Create(data);
            _Response = new Response() { Draw = _Request.Draw };
            _Properties = new List<Property<T>>();
        }

        public DataTablesResult(NameValueCollection data)
        {
            _Request = Request.Create(data);
            _Response = new Response() { Draw = _Request.Draw };
            _Properties = new List<Property<T>>();
        }

        public DataTablesResult(Request request)
        {
            _Request = request;
            _Response = new Response();
            if (_Request != null)
                _Response.Draw = _Request.Draw;
            _Properties = new List<Property<T>>();
        }

        public DataTablesResult<T> AddQuery(IEnumerable<T> query)
        {
            _Query = query.ToList();
            Response.RecordsTotal = _Query.Count;
            Response.RecordsFiltered = _Query.Count;
            return this;
        }

        public DataTablesResult<T> AddProperty<P>(Expression<Func<T, P>> exp, string format = null, bool defaultSort = false)
        {
            _Properties.Add(Property<T>.Create(exp).Format(format).IsDefaultSort(defaultSort));
            return this;
        }

        public Property<T>[] Properties()
        {
            if (_Properties.Count == 0)
            {
                foreach (var pinfo in typeof(T).GetProperties())
                    _Properties.Add(Property<T>.Create(pinfo.Name, pinfo.PropertyType));
            }

            return _Properties.ToArray();
        }

        public DataTablesResult<T> Search()
        {
            if (Request != null && !string.IsNullOrEmpty(Request.Search.Value))
            {
                foreach (Property<T> prop in _Properties)
                {
                    _Query = _Query.Where(x => CheckItem(x, Request.Search.Value)).ToList();
                }
            }

            Response.RecordsFiltered = _Query.Count;

            return this;
        }

        public bool CheckItem(T item, string search)
        {
            foreach (Property<T> p in _Properties)
            {
                if (p.Search(item, search))
                    return true;
            }
            return false;
        }

        public IList<T> GetQuery()
        {
            return _Query;
        }

        public List<SortColumn> GetSortColumns()
        {
            List<SortColumn> result = new List<SortColumn>();
            int index = 0;
            if (Request != null)
            {
                foreach (Order o in Request.Order)
                {
                    result.Add(new SortColumn { Index = o.Column, Direction = (o.Dir == "desc" ? Direction.Descending : Direction.Ascending) });
                    index += 1;
                }
            }
            return result;
        }

        public DataTablesResult<T> AddDefaultSort<TKey>(Func<T, TKey> fn, Direction dir)
        {
            if (dir == Direction.Descending)
                _DefaultSort = x => x.OrderByDescending(fn);
            else
                _DefaultSort = x => x.OrderBy(fn);
            return this;
        }

        public DataTablesResult<T> Sort()
        {
            IOrderedEnumerable<T> ordered = null;
            List<SortColumn> sortColumns = GetSortColumns();
            Property<T>[] props = Properties();
            if (sortColumns.Count > 0)
            {
                int index = 0;
                foreach (SortColumn sc in sortColumns)
                {
                    if (index == 0)
                        ordered = props[sc.Index].OrderBy(_Query, sc.Direction);
                    else
                        ordered = props[sc.Index].ThenOrderBy(ordered, sc.Direction);
                }
            }
            else if (_DefaultSort != null)
                ordered = _DefaultSort(_Query);

            if (ordered != null)
                _Query = ordered.ToList();

            Response.RecordsFiltered = _Query.Count;

            return this;
        }

        public DataTablesResult<T> Page()
        {
            if (Request != null && Request.Length > 0)
                _Page = _Query.Skip(Request.Start).Take(Request.Length).ToList();
            return this;
        }

        public DataTablesResult<T> Fill()
        {
            Search().Sort().Page();
            IList<T> query = (_Page == null) ? _Query : _Page;

            IList<IDictionary<string, object>> data = new List<IDictionary<string, object>>();
            IDictionary<string, object> dict;

            foreach (T item in query)
            {
                dict = new Dictionary<string, object>();
                foreach (Property<T> prop in Properties())
                {
                    dict.Add(prop.Name, prop.ToString(item));
                }
                data.Add(dict);
            }

            Response.Data = data;

            return this;
        }
    }
}
