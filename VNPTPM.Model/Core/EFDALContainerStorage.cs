using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    public class EFDALContainerStorage
    {
        private Dictionary<string, EFDALContainer> storage = new Dictionary<string, EFDALContainer>();

        public void SetEFDbContextForKey(string key, EFDALContainer dalContainer)
        {
            if (storage.ContainsKey(key) == false)
                storage.Add(key, dalContainer);
        }

        public EFDALContainer GetEFDALContainerForKey(string key)
        {
            if (storage.ContainsKey(key))
                return storage[key];
            else
                return null;
        }

        public IEnumerable<EFDALContainer> GetAllDALContainer()
        {
            return storage.Values;
        }
    }
}
