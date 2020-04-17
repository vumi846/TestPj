using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.VM
{
    [Serializable]
    public class UploadVM
    {
        public string AccountID { get; set; }
        public string BranchID { get; set; }
        public string Base64String { get; set; }
        public string FileName { get; set; }
    }
}
