using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VNPTPM.Model.Commons;
using VNPTPM.Model.GlobalResources;

namespace VNPTPM.Model.Partial
{
    [MetadataType(typeof(AD_PAGE_Validation))]
    public partial class AD_PAGE
    {

    }

    [Bind(Exclude = "ID")]
    public class AD_PAGE_Validation
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorRequire")]
        public string ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorRequire")]
        public string Name { get; set; }
    }
}
