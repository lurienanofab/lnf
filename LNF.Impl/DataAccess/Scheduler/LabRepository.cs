using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LNF.Repository.Scheduler;
using NHibernate.Context;

namespace LNF.Impl.DataAccess.Scheduler
{
    public class LabRepository<TContext> : Repository<TContext, Lab>, ILabRepository
        where TContext : ICurrentSessionContext
    {
        public IList<Lab> SelectActive()
        {
            // Despite the Action being called 'SelectAll' the proc only selected where IsActve = 1
            // Note that LabID <> 10 is commented out (without explanation)

            //procLabSelect @Action='SelectAll'

            //SELECT L.BuildingID, B.BuildingName, L.LabID, L.LabName, Room, L.RoomID, L.[Description]
            //FROM dbo.Building B
            //INNER JOIN  dbo.Lab L ON L.BuildingID = B.BuildingID
            //LEFT JOIN sselData.dbo.Room R ON L.RoomID = R.RoomID
            //WHERE L.IsActive = 1--and LabID <> 10-- we don't treat LNF conference room as lab
            //ORDER BY B.BuildingName, LabName

            return Query().Where(x => x.IsActive).ToList();
        }
    }
}
