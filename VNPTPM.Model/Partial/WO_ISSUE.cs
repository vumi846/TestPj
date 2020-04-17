using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VNPTPM.Model.Commons;
using VNPTPM.Model.GlobalResources;

namespace VNPTPM.Model
{
    public partial class WO_ISSUE
    {
        public double Worked { get; set; }
        public System.Guid IssueProgressID { get; set; }
        public string Reporter { get; set; }
        public Nullable<bool> Active { get; set; }
        public string Assignee { get; set; }
        public string AssigneeName { get; set; }
        public string TName { get; set; }
        public string ReporterName { get; set; }
    }
}
