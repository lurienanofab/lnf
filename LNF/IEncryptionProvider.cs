using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF
{
    public interface IEncryptionProvider : ITypeProvider
    {
        string EncryptText(string text);
        string DecryptText(string text);
        string Hash(string input);
    }
}
