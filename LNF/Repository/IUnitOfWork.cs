using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace LNF.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
        void Close();
    }
}
