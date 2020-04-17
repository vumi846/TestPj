using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.VM
{
    public class CheckControlVM
    {
        public string RoleID { get; set; }
        public string PageUrl { get; set; }
        public List<string> Controls { get; set; }
    }
}
