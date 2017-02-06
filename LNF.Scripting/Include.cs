using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LNF.Scripting
{
    public class Include
    {
        public Include(string className, string filePath)
            : this(className, new FileInfo(filePath)) { }

        public Include(string className, FileInfo fileInfo)
        {
            _File = fileInfo;
            _ClassName = className;
        }

        private FileInfo _File;
        private string _ClassName;

        public FileInfo File { get { return _File; } }
        public string ClassName { get { return _ClassName; } }
    }
}
