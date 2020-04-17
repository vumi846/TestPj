using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    public class EFDbContextStorage : IEFDbContextStorage
    {
        private Dictionary<string, EFDbContext> storage = new Dictionary<string, EFDbContext>();

        public void SetEFDbContextForKey(string key, EFDbContext dbContext)
        {
            if (storage.ContainsKey(key) == false)
                storage.Add(key, dbContext);
        }

        public EFDbContext GetEFDbContextForKey(string key)
        {
            if (storage.ContainsKey(key))
                return storage[key];
            else
                return null;
        }

        public IEnumerable<EFDbContext> GetAllDbContexts()
        {
            return storage.Values;
        }
    }
}
