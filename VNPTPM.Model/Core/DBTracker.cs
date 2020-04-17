using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    public delegate void DBSaveChangedEvent(object sender, List<DBChangeTrackerInfo> listDBChanged);
    public class DBTracker
    {
        public DBTracker(EFDbContext dbContext)
        {
            listDBChanged = new List<DBChangeTrackerInfo>();
            foreach (DbEntityEntry db in dbContext.ChangeTracker.Entries())
            {
                listDBChanged.Add(new DBChangeTrackerInfo(db, dbContext.ObjectContext));
            }
        }

        List<DBChangeTrackerInfo> listDBChanged = null;
        public void SaveChanged(object sender)
        {
            if (listDBChanged == null) return;
            if (OnDBSavechanged != null)
                OnDBSavechanged(sender, listDBChanged);
        }

        public static event DBSaveChangedEvent OnDBSavechanged;
    }
}
