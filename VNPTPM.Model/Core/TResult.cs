using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPTPM.Model.Commons;

namespace VNPTPM.Model.Core
{
    public class TResult
    {
        public TResult()
        {
            Status = (short)EStatus.Ok;
        }

        public short Status { get; set; }
        public object Data { get; set; }
        public string Msg { get; set; }
    }
}
