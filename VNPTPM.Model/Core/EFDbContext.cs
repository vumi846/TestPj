using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    public class EFDbContext : DbContext, IDbModelCacheKeyProvider
    {
        public string SchemaName { get; private set; }

        public EFDbContext(ObjectContext objectContext, string schema)
            : base(objectContext, true)
        {
            SchemaName = schema;
            objectContext.DefaultContainerName = "VNPTPMEntities";
            //if (DBName.Equals(EFDALContainer.DbSystemName, StringComparison.OrdinalIgnoreCase))
            //    IsSysConnect = true;
            //else
            //    IsSysConnect = false;

            Configuration.AutoDetectChangesEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }

        public EFDbContext(string name)
            : base(name)
        {
            SchemaName = "dbo";

            Configuration.AutoDetectChangesEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }

        public System.Data.Entity.Core.Objects.ObjectContext ObjectContext
        {
            get
            {
                return ((IObjectContextAdapter)this).ObjectContext;
            }
        }

        public static Expression<Func<T, U>> BuildLambda<T, U>(PropertyInfo property)
        {
            var param = Expression.Parameter(typeof(T), "p");
            MemberExpression memberExpression = Expression.Property(param, property);
            var lambda = Expression.Lambda<Func<T, U>>(memberExpression, param);
            return lambda;
        }

        public static void SetPrecision<T>(EntityTypeConfiguration<T> entityConfig, PropertyInfo property, byte precision, byte scale) where T : class
        {
            var lambda = BuildLambda<T, decimal>(property);
            entityConfig.Property(lambda).HasPrecision(precision, scale);
        }

        public void SetEntity<T>(DbModelBuilder modelBuilder, string tableName) where T : class
        {
            modelBuilder.Entity<T>();
        }

        public string CacheKey
        {
            get { return this.SchemaName; }
        }
    }
}
