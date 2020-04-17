using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    public interface IEFDbContextStorage
    {
        EFDbContext GetEFDbContextForKey(string key);

        void SetEFDbContextForKey(string key, EFDbContext dbContext);

        IEnumerable<EFDbContext> GetAllDbContexts();
    }
}
