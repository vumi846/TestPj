using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    public interface IRepository
    {
        void AcceptAllChanges();

        void Attach<TEntity>(TEntity entity) where TEntity : class;

        void Add<TEntity>(TEntity entity) where TEntity : class;

        void Update<TEntity>(TEntity entity) where TEntity : class;

        void AddOrUpdate<TEntity>(params TEntity[] entities) where TEntity : class;

        void Delete<TEntity>(TEntity entity) where TEntity : class;

        void Delete<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        TEntity GetByKey<TEntity>(object keyValue) where TEntity : class;

        IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        TEntity GetOne<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        TEntity GetOneByKey<TEntity>(object item, bool IsGetFromDataSource) where TEntity : class;

        IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class;

        int Count<TEntity>() where TEntity : class;

        int Count<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        int Max<TEntity>(Expression<Func<TEntity, int>> criteria) where TEntity : class;

        decimal Max<TEntity>(Expression<Func<TEntity, decimal>> criteria) where TEntity : class;

        float Max<TEntity>(Expression<Func<TEntity, float>> criteria) where TEntity : class;

        double Max<TEntity>(Expression<Func<TEntity, double>> criteria) where TEntity : class;

        long Max<TEntity>(Expression<Func<TEntity, long>> criteria) where TEntity : class;

        short Max<TEntity>(Expression<Func<TEntity, short>> criteria) where TEntity : class;

        IUnitOfWork UnitOfWork { get; }

        IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class;

        IQueryable<TEntity> GetQuery<TEntity>(params string[] tables) where TEntity : class;

        System.Data.Entity.Core.Objects.ObjectResult<TEntity> ExecuteSQL<TEntity>(string sql) where TEntity : class;

        System.Data.Entity.Core.Objects.ObjectResult<TEntity> ExecuteSQL<TEntity>(string sql, object[] parameters) where TEntity : class;

        System.Data.DataSet ExecuteStoreScalar(string sqlName, object[] parameterValues);

        int ExecuteStoreOutParas(string sqlName, object[] parameterValues);

        IQueryable<TEntity> EntitySQL<TEntity>(IQueryable<TEntity> source, string sql) where TEntity : class;

        IQueryable<TEntity> EntitySQL<TEntity>(IQueryable<TEntity> source, string sql, object[] parameters) where TEntity : class;

        bool IsExits<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        string[] GetKeys<TEntity>(TEntity entity) where TEntity : class;

        int GetPageOfSourceByKey<TEntity>(IQueryable<TEntity> source, string Key, string Value, int PageSize, string Sort);

        string GetDatabaseName();

        void Close();
    }
}
