using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.VM
{
    [Serializable]
    public class SearchVM
    {
        public string BranchIDList { get; set; }
        public string MethodIDList { get; set; }
        public string PackageIDList { get; set; }
        public Guid AccountID { get; set; }
        public string SearchTxt { get; set; }
        public string DisTypeTxt { get; set; }
        public string StatusTxt { get; set; }
        public string Merchandises { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
    }
}
