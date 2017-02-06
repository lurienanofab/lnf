using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.CommonTools;

namespace LNF.Scripting
{
    public class Tools
    {
        private WriteData _WriteData;

        public WriteData WriteData { get { return _WriteData; } }

        public string GetVersion()
        {
            return Utility.Version();
        }

        public Tools()
        {
            _WriteData = new WriteData();
        }
    }
}
