using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.CommonTools
{
    public static class GenericEqualityComparer
    {
        public static IEqualityComparer<T> CreateEqualityComparer<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            return new GenericEqualityComparer<T>(equals, getHashCode);
        }

        public static IEqualityComparer<T> CreateEqualityComparer<T>(this IEnumerable<T> enumerable, Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            return CreateEqualityComparer(equals, getHashCode);
        }
    }

    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> _equals;
        private Func<T, int> _getHashCode;

        public GenericEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            _equals = equals;
            _getHashCode = getHashCode;
        }

        public bool Equals(T x, T y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCode(obj);
        }
    }
}
