using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using VNPTPM.Model.Commons;
using VNPTPM.Model.GlobalResources;

namespace VNPTPM.Model.VM
{
    [Serializable]
    public class ChangePasswordVM
    {
        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "MsgUserNameNotEmpty")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "MsgPasswordNotEmpty")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "MsgNewPasswordNotEmpty")]
        public string NewPassword { get; set; }
        
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "MsgNewPasswordNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}
