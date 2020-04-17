using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPTPM.Model.Core;

namespace VNPTPM.Model.Commons
{
    public class VNPTLogs
    {
        public Guid Write(IRepository repository, string content)
        {
            var id = Guid.NewGuid();
            repository.Add(new AD_LOG()
            {
                ID = id,
                ServiceName = VNPTHelper.GetServiceName(),
                UserName = VNPTHelper.GetUserName(),
                CreateAt = DateTime.Now,
                Data = content
            });

            //repository.UnitOfWork.SaveChanges();
            return id;
        }

        public void Write(IRepository repository, EAction action, string content)
        {
            repository.Add(new AD_LOG()
            {
                ID = Guid.NewGuid(),
                ServiceName = VNPTHelper.GetServiceName(),
                UserName = VNPTHelper.GetUserName(),
                ActionRec = (int)action,
                CreateAt = DateTime.Now,
                Data = content
            });
        }
    }
}
