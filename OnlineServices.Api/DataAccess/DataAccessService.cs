using LNF.DataAccess;
using System;
using System.Data.Common;

namespace OnlineServices.Api.DataAccess
{
    public class DataAccessService : IDataAccessService
    {
        public ISession Session => throw new NotImplementedException();

        public string UniversalPassword => throw new NotImplementedException();

        public bool ShowSql => throw new NotImplementedException();

        public bool IsProduction()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork StartUnitOfWork()
        {
            throw new NotImplementedException();
        }

        public DbConnection CreateConnection(string connstr)
        {
            throw new NotImplementedException();
        }
    }
}
