using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LNF.DataTables
{
    //public interface IProperty<T>
    //{
    //    string Name { get; }
    //    string ToString(T item);
    //    IOrderedEnumerable<T> OrderBy(IList<T> query, Direction dir);
    //    IOrderedEnumerable<T> ThenOrderBy(IOrderedEnumerable<T> query, Direction dir);
    //    bool Search(T item, string text);
    //}

    public class Property<T> //: IProperty<T>
    {
        private string _Name;
        private bool _DefaultSort;
        private Type _PropertyType;

        //private Expression<Func<T, object>> _GetValue;

        private string _StringFormatter;
        private Func<IList<T>, IOrderedEnumerable<T>> _OrderByAscending { get; set; }
        private Func<IList<T>, IOrderedEnumerable<T>> _OrderByDescending { get; set; }
        private Func<IOrderedEnumerable<T>, IOrderedEnumerable<T>> _ThenOrderByAscending { get; set; }
        private Func<IOrderedEnumerable<T>, IOrderedEnumerable<T>> _ThenOrderByDescending { get; set; }

        public string Name { get { return _Name; } }
        public bool DefaultSort { get { return _DefaultSort; } }
        public Type PropertyType { get { return _PropertyType; } }

        //internal Property(Expression<Func<T, object>> exp, Type t)
        //{
        //    _Name = LNF.CommonTools.Utility.PropertyName(exp);
        //    Func<T, object> fn = exp.Compile();
        //    _OrderByAscending = x => x.OrderBy(fn);
        //    _OrderByDescending = x => x.OrderByDescending(fn);
        //    _ThenOrderByAscending = x => x.ThenBy(fn);
        //    _ThenOrderByDescending = x => x.ThenByDescending(fn);
        //}

        private Property() { }

        public static Property<T> Create<TKey>(Expression<Func<T, TKey>> exp)
        {
            Property<T> prop = new Property<T>();
            prop._Name = LNF.CommonTools.Utility.PropertyName(exp);
            prop._PropertyType = typeof(TKey);
            prop._DefaultSort = false;

            Func<T, TKey> fn = exp.Compile();

            prop._OrderByAscending = x => x.OrderBy(fn);
            prop._OrderByDescending = x => x.OrderByDescending(fn);
            prop._ThenOrderByAscending = x => x.ThenBy(fn);
            prop._ThenOrderByDescending = x => x.ThenByDescending(fn);

            return prop;
        }

        public static Property<T> Create(string propertyName, Type propertyType)
        {
            Property<T> prop = new Property<T>();
            prop._Name = propertyName;
            prop._PropertyType = propertyType;
            prop._DefaultSort = false;

            Func<T, object> fn = x => typeof(T).GetProperty(propertyName).GetValue(x, null);

            prop._OrderByAscending = x => x.OrderBy(fn);
            prop._OrderByDescending = x => x.OrderByDescending(fn);
            prop._ThenOrderByAscending = x => x.ThenBy(fn);
            prop._ThenOrderByDescending = x => x.ThenByDescending(fn);

            return prop;
        }

        public object GetValue(T item)
        {
            return typeof(T).GetProperty(_Name).GetValue(item, null);
        }

        public Property<T> Format(string format)
        {
            _StringFormatter = format;
            return this;
        }

        public Property<T> IsDefaultSort(bool isDefault)
        {
            _DefaultSort = isDefault;
            return this;
        }

        public IOrderedEnumerable<T> OrderBy(IList<T> query, Direction dir)
        {
            if (dir == Direction.Descending)
                return _OrderByDescending.Invoke(query);
            else
                return _OrderByAscending.Invoke(query);
        }

        public IOrderedEnumerable<T> ThenOrderBy(IOrderedEnumerable<T> query, Direction dir)
        {
            if (dir == Direction.Descending)
                return _ThenOrderByDescending(query);
            else
                return _ThenOrderByAscending(query);
        }

        public string ToString(T item)
        {
            object val = GetValue(item);
            if (val == null) return string.Empty;
            if (_StringFormatter != null)
            {
                try
                {
                    return string.Format(_StringFormatter, val);
                }
                catch
                {
                    return val.ToString();
                }
            }
            else
                return val.ToString();
        }

        public bool Search(T item, string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;
            string val = ToString(item);
            if (string.IsNullOrEmpty(val))
                return false;
            return val.ToLower().Contains(text.ToLower());
        }
    }
}
