using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VNPTPM.Model.Core
{
    public class EFDALContainerManager
    {
        private const string STORAGE_KEY = "EFDALContainerSession";

        public static EFDALContainer GetEFDALContainerForKey(string key)
        {
            EFDALContainer dalContainer = null;
            HttpContext context = HttpContext.Current;

            if (context != null && context.Session != null)
            {
                if (context.Session[STORAGE_KEY] is EFDALContainerStorage ctxStorage)
                    dalContainer = ctxStorage.GetEFDALContainerForKey(key);
            }

            return dalContainer;
        }

        public static void AddEFDALContainerToStore(string key, EFDALContainer dalContainer)
        {
            HttpContext context = HttpContext.Current;

            if (context != null && context.Session != null)
            {
                if (!(context.Session[STORAGE_KEY] is EFDALContainerStorage ctxStorage))
                    ctxStorage = new EFDALContainerStorage();

                ctxStorage.SetEFDbContextForKey(key, dalContainer);
                context.Session[STORAGE_KEY] = ctxStorage;
            }
        }

        public static IEnumerable<EFDALContainer> GetAllEFDALContainer()
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                EFDALContainerStorage ctxStorage = context.Session[STORAGE_KEY] as EFDALContainerStorage;
                return ctxStorage.GetAllDALContainer();
            }
            else
                return null;
        }

        public static void CloseAllDALContainer()
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                if (context.Session[STORAGE_KEY] is EFDALContainerStorage ctxStorage && context.Session != null)
                {
                    foreach (EFDALContainer ctx in ctxStorage.GetAllDALContainer())
                    {
                        ctx.Close();
                        ctx.Dispose();
                    }

                    context.Session.Remove(STORAGE_KEY);
                }
            }
        }
    }
}
