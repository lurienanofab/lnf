using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF
{
    public interface ISerializationProvider : ITypeProvider
    {
        ISerializer Xml { get; }
        ISerializer Json { get; }
    }
}
