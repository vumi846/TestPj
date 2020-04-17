using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VNPTPM.Model.Core
{
    public class EFDbContextManager
    {
        private const string STORAGE_KEY = "HTTPContextSession";

        public static EFDbContext GetEFDbContextForKey(string key)
        {
            EFDbContext dbContext = null;
            var context = HttpContext.Current;

            if (context != null && context.Session != null)
            {
                if (context.Session[STORAGE_KEY] is EFDbContextStorage ctxStorage)
                    dbContext = ctxStorage.GetEFDbContextForKey(key);
            }

            return dbContext;
        }

        public static void AddEFDbContextToStore(string key, EFDbContext dbContext)
        {
            HttpContext context = HttpContext.Current;

            if (context != null && context.Session != null)
            {
                if (!(context.Session[STORAGE_KEY] is EFDbContextStorage ctxStorage))
                    ctxStorage = new EFDbContextStorage();

                ctxStorage.SetEFDbContextForKey(key, dbContext);
                context.Session[STORAGE_KEY] = ctxStorage;
            }
        }

        public static IEnumerable<EFDbContext> GetAllEFDbContext()
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                EFDbContextStorage ctxStorage = context.Session[STORAGE_KEY] as EFDbContextStorage;
                return ctxStorage.GetAllDbContexts();
            }
            else
                return null;
        }

        public static void CloseAllDbContext()
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                if (context.Session[STORAGE_KEY] is EFDbContextStorage ctxStorage)
                {
                    foreach (EFDbContext ctx in ctxStorage.GetAllDbContexts())
                    {
                        if (ctx.Database.Connection.State == System.Data.ConnectionState.Open)
                            ctx.Database.Connection.Close();

                        if (ctx.ObjectContext.Connection.State == System.Data.ConnectionState.Open)
                            ctx.Database.Connection.Close();

                        ctx.ObjectContext.Dispose();
                        ctx.Dispose();
                    }

                    context.Session.Remove(STORAGE_KEY);
                }
            }
        }

        /// <summary>
        /// Close all db context
        /// </summary>
        public static void CloseAllDbContext(IDALContainer dalContainer)
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                if (context.Session[STORAGE_KEY] is EFDbContextStorage ctxStorage)
                {
                    foreach (EFDbContext ctx in ctxStorage.GetAllDbContexts())
                    {
                        if (ctx.Database.Connection.State == System.Data.ConnectionState.Open)
                            ctx.Database.Connection.Close();

                        ctx.Dispose();
                    }

                    context.Session.Remove(STORAGE_KEY);
                }
            }
            else if (dalContainer != null)
            {
                dalContainer.Close();
                dalContainer.Dispose();
            }
        }
    }
}
