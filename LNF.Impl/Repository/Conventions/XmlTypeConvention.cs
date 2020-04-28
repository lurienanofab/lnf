using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data.Common;
using System.Xml.Linq;

namespace LNF.Impl.Repository.Conventions
{
    public class XElementTypeConvention : IUserTypeConvention
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Type == typeof(XElement));
        }

        public void Apply(IPropertyInstance instance)
        {
            instance.CustomType<XElementType>();
        }
    }

    public class XElementType : IUserType
    {
        public new bool Equals(object x, object y)
        {
            if (x == null || y == null)
                return false;

            var el1 = (XElement)x;
            var el2 = (XElement)y;

            if (el1 == null && el2 == null)
                return true;

            if (el1 == null || el2 == null)
                return false;

            return el1.ToString() == el2.ToString();
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            if (names.Length != 1)
                throw new InvalidOperationException("names array has more than one element. can't handle this!");

            var val = Convert.ToString(rs[names[0]]);

            if (!string.IsNullOrEmpty(val))
                return XElement.Parse(val);

            return null;
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            var parameter = (DbParameter)cmd.Parameters[index];

            if (value == null)
            {
                parameter.Value = DBNull.Value;
                return;
            }

            parameter.Value = ((XElement)value).ToString();
        }

        public object DeepCopy(object value)
        {
            if (!(value is XElement toCopy))
                return null;

            var copy = new XElement(toCopy);

            return copy;
        }

        public object Assemble(object cached, object owner)
        {
            var val = Convert.ToString(cached);

            if (!string.IsNullOrEmpty(val))
                return XElement.Parse(val);
            else
                return null;
        }

        public object Disassemble(object value)
        {
            if (value is XElement val)
                return val.ToString();
            else
                return null;
        }

        public SqlType[] SqlTypes
        {
            get { return new[] { new XmlSqlType() }; }
        }

        public Type ReturnedType
        {
            get { return typeof(XElement); }
        }

        public bool IsMutable
        {
            get { return true; }
        }

        public object Replace(object original, object target, object owner)
        {
            return DeepCopy(original);
        }
    }
}
