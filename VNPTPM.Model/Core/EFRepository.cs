using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    public class EFRepository : IRepository
    {
        private const string ENTITY_KEY_NAME = "RecID";
        private IUnitOfWork _UnitOfWork = null;
        private EFDbContext _DbContext = null;
        private System.Data.Entity.Core.Objects.ObjectContext _objectContext { get { return _DbContext.ObjectContext; } }
        private string ConnectString
        {
            get
            {
                DbConnection enCnn = (((IObjectContextAdapter)_DbContext).ObjectContext.Connection as System.Data.Entity.Core.EntityClient.EntityConnection).StoreConnection;
                return enCnn.ConnectionString;
            }
        }
        public EFRepository(IUnitOfWork Uow)
        {
            _DbContext = (EFDbContext)Uow.Orm;

        }

        public void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();
            dbContext.Set<TEntity>().Attach(entity);
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();
            var ent = dbContext.Set<TEntity>().Add(entity);
            Console.WriteLine(ent.ToString());
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();
            System.Data.Entity.Core.Objects.ObjectContext objContext = dbContext.ObjectContext;

            var fqen = GetEntityName<TEntity>();
            System.Data.Entity.Core.EntityKey key = objContext.CreateEntityKey(fqen, entity);

            object originalItem;
            if (objContext.TryGetObjectByKey(key, out originalItem))
            {
                System.Data.Entity.Core.EntityKeyMember[] keys = key.EntityKeyValues;

                foreach (System.Data.Entity.Core.EntityKeyMember keyMember in keys)
                {
                    PropertyInfo entityKeyProperty = entity.GetType().GetProperty(keyMember.Key);
                    PropertyInfo originalKeyProperty = originalItem.GetType().GetProperty(keyMember.Key);
                    if (entityKeyProperty != null && originalKeyProperty != null)
                    {
                        object originalKeyValue = originalKeyProperty.GetValue(originalItem, null);
                        entityKeyProperty.SetValue(entity, originalKeyValue, null);
                    }
                }

                objContext.ApplyCurrentValues<TEntity>(key.EntitySetName, entity);
            }
        }

        public void AddOrUpdate<TEntity>(params TEntity[] entities) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();
            dbContext.Set<TEntity>().AddOrUpdate(entities);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();
            dbContext.Set<TEntity>().Attach(entity);
            dbContext.Set<TEntity>().Remove(entity);
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            var records = GetQuery<TEntity>().Where(criteria)
                .ToArray();

            for (int i = 0; i < records.Length; i++)
            {
                Delete<TEntity>(records[i]);
            }
        }

        public TEntity GetByKey<TEntity>(object keyValue) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();
            System.Data.Entity.Core.Objects.ObjectContext objContext = dbContext.ObjectContext;

            System.Data.Entity.Core.EntityKey key = GetEntityKey<TEntity>(keyValue);

            if (objContext.TryGetObjectByKey(key, out object originalItem))
            {
                return (TEntity)originalItem;
            }
            return default(TEntity);
        }

        private EntityKey GetEntityKey<TEntity>(object keyValue) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();
            ObjectContext objContext = dbContext.ObjectContext;

            var entitySetName = GetEntityName<TEntity>();
            var objectSet = objContext.CreateObjectSet<TEntity>();
            var keyPropertyName = objectSet.EntitySet.ElementType.KeyMembers[0].ToString();
            var qualifiedEntitySetName = objContext.DefaultContainerName + "." + entitySetName;
            var entityKey = new EntityKey(qualifiedEntitySetName, keyPropertyName, keyValue);
            return entityKey;
        }

        public TEntity GetOneByKey<TEntity>(object item, bool IsGetFromDataSource) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();
            ObjectContext objContext = dbContext.ObjectContext;

            object originalItem = null;

            var entitySet = objContext.CreateObjectSet<TEntity>();
            EntityKey key = objContext.CreateEntityKey(GetEntityName<TEntity>(), item);

            if (IsGetFromDataSource)
            {
                entitySet.MergeOption = MergeOption.OverwriteChanges;
                try
                {
                    string predicate = "";
                    List<ObjectParameter> parameters = new List<ObjectParameter>();
                    foreach (var k in key.EntityKeyValues)
                    {
                        predicate = predicate + (predicate == "" ? "" : "&&") + entitySet.Name + "." + k.Key + "==@" + k.Key;
                        parameters.Add(new ObjectParameter(k.Key, k.Value));
                    }

                    TEntity objectReturn = (TEntity)entitySet.Where(predicate, parameters.ToArray()).Execute(MergeOption.OverwriteChanges).FirstOrDefault();
                    //if(objectReturn!=null)
                    //    this._ObjectContext.ApplyCurrentValues<TEntity>(GetEntityName<TEntity>(), objectReturn);
                    return objectReturn;
                }
                finally { }
            }
            else
                if (objContext.TryGetObjectByKey(key, out originalItem))
            {
                return (TEntity)originalItem;
            }
            return default(TEntity);
        }

        public IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            return GetQuery<TEntity>().Where(criteria);
        }

        public TEntity GetOne<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            return Get<TEntity>(criteria).FirstOrDefault();
        }

        public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return GetQuery<TEntity>().AsEnumerable<TEntity>();
        }

        public int Count<TEntity>() where TEntity : class
        {
            return GetQuery<TEntity>().Count();
        }

        public int Count<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            return GetQuery<TEntity>().Count(criteria);
        }

        public int Max<TEntity>(Expression<Func<TEntity, int>> criteria) where TEntity : class
        {
            return GetQuery<TEntity>().Max(criteria);
        }

        public decimal Max<TEntity>(Expression<Func<TEntity, decimal>> criteria) where TEntity : class
        {
            return GetQuery<TEntity>().Max(criteria);
        }
        public float Max<TEntity>(Expression<Func<TEntity, float>> criteria) where TEntity : class
        {
            return GetQuery<TEntity>().Max(criteria);
        }
        public double Max<TEntity>(Expression<Func<TEntity, double>> criteria) where TEntity : class
        {
            return GetQuery<TEntity>().Max(criteria);
        }
        public long Max<TEntity>(Expression<Func<TEntity, long>> criteria) where TEntity : class
        {
            return GetQuery<TEntity>().Max(criteria);
        }

        public short Max<TEntity>(Expression<Func<TEntity, short>> criteria) where TEntity : class
        {
            return GetQuery<TEntity>().Max(criteria);
        }

        private string GetEntityName<TEntity>()
        {
            return typeof(TEntity).Name;
        }

        private static Dictionary<string, Type> cType = new Dictionary<string, Type>();

        public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();

            return dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetQuery<TEntity>(params string[] tables) where TEntity : class
        {
            var context = GetQuery<TEntity>();
            if (tables != null)
            {
                foreach (string table in tables)
                {
                    context = ((DbQuery<TEntity>)context).Include(table);
                }
            }

            return context;
        }

        public IQueryable<TEntity> EntitySQL<TEntity>(IQueryable<TEntity> source, string sql) where TEntity : class
        {
            ObjectQuery<TEntity> objQuery = (ObjectQuery<TEntity>)source;

            return objQuery.Where(sql);
        }

        public IQueryable<TEntity> EntitySQL<TEntity>(IQueryable<TEntity> source, string sql, object[] parameters) where TEntity : class
        {
            ObjectQuery<TEntity> objQuery = (ObjectQuery<TEntity>)source;

            ObjectParameter[] prs = new ObjectParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                prs[i] = new ObjectParameter("p" + i, parameters[i]);
            }

            return objQuery.Where(sql, prs);
        }

        public ObjectResult<TEntity> ExecuteSQL<TEntity>(string sql) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();
            return ((IObjectContextAdapter)dbContext).ObjectContext.ExecuteStoreQuery<TEntity>(sql);
        }

        public ObjectResult<TEntity> ExecuteSQL<TEntity>(string sql, object[] parameters) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();
            return ((IObjectContextAdapter)dbContext).ObjectContext.ExecuteStoreQuery<TEntity>(sql, parameters);
        }

        public System.Data.DataSet ExecuteStoreScalar(string sqlName, object[] parameterValues)
        {
            System.Data.DataSet dtReturn = new System.Data.DataSet();

            using (var conn = new SqlConnection(this.ConnectString))
            {
                // Create Command
                var cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = sqlName;
                cmd.CommandTimeout = 0;
                // Open Connection
                conn.Open();

                // Discover Parameters for Stored Procedure
                // Populate command.Parameters Collection.
                // Causes Rountrip to Database.
                SqlCommandBuilder.DeriveParameters(cmd);
                // Initialize Index of parameterValues Array
                int index = 0;
                // Populate the Input Parameters With Values Provided        
                foreach (SqlParameter parameter in cmd.Parameters)
                {
                    if (parameter.Direction == System.Data.ParameterDirection.Input ||
                         parameter.Direction == System.Data.ParameterDirection.
                                                     InputOutput)
                    {
                        if (parameterValues.Count() <= index || parameterValues[index] == null)
                        {
                            parameter.Value = DBNull.Value;
                        }
                        else
                            parameter.Value = parameterValues[index];
                        index++;
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dtReturn);
            }
            return dtReturn;
        }

        public int ExecuteStoreOutParas(string sqlName, object[] parameterValues)
        {
            int iReturn = 0;
            using (var conn = new SqlConnection(ConnectString))
            {
                // Open Connection
                conn.Open();

                SqlTransaction transaction;
                // Start a local transaction.
                transaction = conn.BeginTransaction();

                // Create Command
                var cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = sqlName;
                cmd.CommandTimeout = 0;
                cmd.Transaction = transaction;

                Hashtable hsOut = new Hashtable();
                // Discover Parameters for Stored Procedure
                // Populate command.Parameters Collection.
                // Causes Rountrip to Database.
                SqlCommandBuilder.DeriveParameters(cmd);
                // Initialize Index of parameterValues Array
                int index = 0;
                int i = 0;
                // Populate the Input Parameters With Values Provided        
                try
                {
                    foreach (SqlParameter parameter in cmd.Parameters)
                    {
                        if (parameter.Direction == System.Data.ParameterDirection.Input ||
                             parameter.Direction == System.Data.ParameterDirection.
                                                         InputOutput)
                        {
                            if (parameterValues.Count() <= index || parameterValues[index] == null)
                            {
                                parameter.Value = DBNull.Value;
                            }
                            else
                                parameter.Value = parameterValues[index];

                            if (parameter.Direction == System.Data.ParameterDirection.InputOutput)
                                hsOut.Add(index, i);

                            index++;
                        }
                        i++;
                    }

                    iReturn = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //ExceptionUtility.LogException(ex, "error");
                    throw ex;
                }
                finally
                {
                    // Attempt to commit the transaction.
                    //iReturn = (int)cmd.Parameters["@RETURNVALUE"].Value;
                    //if (iReturn == 0)
                    //{
                    transaction.Commit();
                    foreach (DictionaryEntry item in hsOut)
                    {
                        index = (int)item.Key;
                        i = (int)item.Value;
                        parameterValues[index] = cmd.Parameters[i].Value;
                    }
                    //}
                    //else
                    //{
                    //    transaction.Rollback();
                    //    hsOut.Clear();
                    //}

                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                }
            }
            return iReturn;
        }

        public bool IsExits<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            return GetQuery<TEntity>().Any(criteria);
        }

        public void AcceptAllChanges()
        {
            _objectContext.AcceptAllChanges();
        }

        public string GetDatabaseName()
        {
            //return _ObjectContext.Connection.Database;
            return this._DbContext.Database.Connection.Database;
        }

        public string[] GetKeys<TEntity>(TEntity entity) where TEntity : class
        {
            EFDbContext dbContext = GetEFDbContext<TEntity>();

            System.Data.Entity.Core.EntityKey key = ((IObjectContextAdapter)dbContext).ObjectContext.CreateEntityKey(GetEntityName<TEntity>(), entity);

            if (key != null)
            {
                return key.EntityKeyValues.Select(o => o.Key).ToArray();
            }

            return new string[] { };
        }

        public EFDbContext GetEFDbContext<TEntity>()
        {
            return _DbContext;
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (_UnitOfWork == null)
                {
                    _UnitOfWork = new EFUnitOfWork(_DbContext);
                }
                return _UnitOfWork;
            }
        }

        public int GetPageOfSourceByKey<TEntity>(IQueryable<TEntity> source, string Key, string Value, int PageSize, string Sort)
        {
            if (source == null) return 0;

            DbQuery<TEntity> dbQuery = ((DbQuery<TEntity>)source);
            var internalQueryField = typeof(DbQuery<TEntity>).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(f => f.Name.Equals("_internalQuery"));
            var internalQuery = internalQueryField.GetValue(dbQuery);
            var objectQueryField = internalQuery.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(f => f.Name.Equals("_objectQuery"));
            if (objectQueryField == null)
            {
                objectQueryField = internalQuery.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(f => f.Name.Equals("_objectQuery"));
            }
            // Here's your ObjectQuery!

            if (!(objectQueryField.GetValue(internalQuery) is ObjectQuery<TEntity> objQuery)) return 0;

            string command = objQuery.ToTraceString();

            string SQL = "SELECT @PageCurrent = ((RowIndex-1)/" + PageSize.ToString() + ")+1 "
                      + "FROM (SELECT Row_Number() over (order by " + Sort + ") as RowIndex," + Key + " FROM (" + command + ") sql1) sql "
                      + "WHERE " + Key + " = '" + Value + "'";

            try
            {
                using (var conn = new SqlConnection(ConnectString))
                {
                    // Create Command
                    var cmd = conn.CreateCommand();
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = SQL;

                    conn.Open();

                    cmd.Parameters.Add(new SqlParameter() { Direction = System.Data.ParameterDirection.InputOutput, ParameterName = "PageCurrent", SqlDbType = System.Data.SqlDbType.Int, Value = 1 });
                    cmd.ExecuteNonQuery();
                    return (int)cmd.Parameters[0].Value;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_DbContext != null)
                {
                    _DbContext.Dispose();
                    _DbContext = null;
                }
            }
        }

        public void Close()
        {
            if (_DbContext.Database.Connection.State == System.Data.ConnectionState.Open)
                _DbContext.Database.Connection.Close();
        }
        
        public bool IsRelationShip
        {
            get;
            set;
        }
    }
}
