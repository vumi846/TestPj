using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    [Serializable]
    public class DBChangeTrackerInfo
    {
        public DBChangeTrackerInfo() { }

        public DBChangeTrackerInfo(DbEntityEntry dbEntity, System.Data.Entity.Core.Objects.ObjectContext objContext)
        {
            entity = dbEntity.Entity;
            currentValues = dbEntity.State == System.Data.Entity.EntityState.Deleted ? null : (DbPropertyValues)dbEntity.CurrentValues.Clone();
            originalValues = dbEntity.State == System.Data.Entity.EntityState.Added ? null : (DbPropertyValues)dbEntity.OriginalValues.Clone();
            state = dbEntity.State;
            if (entity == null) return;
            #region GetEntityKeyValue
            string tblN = entity.GetType().Name;
            System.Data.Entity.Core.EntityKey key = objContext.CreateEntityKey(tblN, entity);
            DbKeyMember[] km = new DbKeyMember[key.EntityKeyValues.Length];
            for (int i = 0; i < key.EntityKeyValues.Length; i++)
                km[i] = new DbKeyMember(key.EntityKeyValues[i].Key, key.EntityKeyValues[i].Value);
            entityKey = new DbEntityKey(tblN, km);
            //Tracking changed data
            if (state == System.Data.Entity.EntityState.Modified && originalValues != null)
            {
                listRecordChanged = new List<DbRecordChangedInfo>();
                foreach (var fieldProperty in originalValues.PropertyNames)
                {
                    if (dbEntity.Property(fieldProperty).IsModified)
                    {
                        listRecordChanged.Add(new DbRecordChangedInfo(fieldProperty, originalValues[fieldProperty], currentValues[fieldProperty]));
                    }
                }
            }
            #endregion
        }

        string userName = "";
        public string UserName { get { return userName; } set { userName = value; } }

        DbEntityKey entityKey;
        public DbEntityKey EntityKey { get { return entityKey; } set { entityKey = value; } }

        object entity = null;
        public object Entity { get { return entity; } set { entity = value; } }

        DbPropertyValues currentValues;
        public DbPropertyValues CurrentValues { get { return currentValues; } set { currentValues = value; } }

        DbPropertyValues originalValues;
        public DbPropertyValues OriginalValues { get { return originalValues; } set { originalValues = value; } }

        System.Data.Entity.EntityState state;
        public System.Data.Entity.EntityState State { get { return state; } set { state = value; } }

        List<DbRecordChangedInfo> listRecordChanged;
        public List<DbRecordChangedInfo> ListRecordChanged { get { return listRecordChanged; } set { listRecordChanged = value; } }

    }
}
