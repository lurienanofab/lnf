using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace LNF.Repository
{
    [Serializable]
    public class Bytes : IEnumerable<byte>
    {
        private IEnumerable<byte> _value;

        public int Length { get; }

        public Bytes(IEnumerable<byte> value)
        {
            _value = value;
            Length = _value.Count();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var arr = obj as byte[];

            if (arr == null) return false;

            return Enumerable.SequenceEqual(arr, _value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return ToString("x2");
        }

        public string ToString(string format)
        {
            if (_value == null) return null;
            if (Length == 0) return string.Empty;

            StringBuilder hex = new StringBuilder(Length * 2);

            foreach (byte b in _value)
                hex.Append(b.ToString(format));

            return "0x" + hex.ToString();
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return _value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
