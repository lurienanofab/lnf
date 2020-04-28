using LNF.Repository;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data.Common;
using System.Linq;

namespace LNF.Impl.Repository.Conventions
{
    public class BytesUserType : IUserType
    {
        private static readonly SqlType[] _sqlTypes = { NHibernateUtil.Binary.SqlType };

        public SqlType[] SqlTypes => _sqlTypes;

        public Type ReturnedType => typeof(Bytes);

        public bool IsMutable => false;

        public object Assemble(object cached, object owner)
        {
            return DeepCopy(cached);
        }

        public object DeepCopy(object value)
        {
            if (value == null) return null;

            if (!(value is Bytes b)) return null;

            return new Bytes(b.ToArray());
        }

        public object Disassemble(object value)
        {
            return DeepCopy(value);
        }

        public new bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

        public int GetHashCode(object x)
        {
            if (x == null) return 0;
            return x.GetHashCode();
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            object obj = NHibernateUtil.Binary.NullSafeGet(rs, names[0], session);
            if (obj == null) return null;
            var value = (byte[])obj;
            return new Bytes(value);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
                cmd.Parameters[index].Value = DBNull.Value;
            else
            {
                Bytes b = (Bytes)value;
                cmd.Parameters[index].Value = b.ToArray();
            }
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }
    }
}
