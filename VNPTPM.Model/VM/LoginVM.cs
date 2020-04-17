using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPTPM.Model.Commons;
using VNPTPM.Model.GlobalResources;

namespace VNPTPM.Model.VM
{
    public class LoginVM
    {
        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = VNPTResources.ID.Login_PhoneNotNull)]
        [RegularExpression(@"^(([0-9]*)|(([0-9]*)\.([0-9]*)))$", ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "MsgErrorRegularExpressionNumber")]
        [MinLength(10, ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = VNPTResources.ID.Login_PhoneMinLength)]
        public string Phone { get; set; }
        public string DeviceName { get; set; }
        public string IMEICode { get; set; }
    }

    public class OTPVM
    {
        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = VNPTResources.ID.Login_IDNotNull)]
        public Guid ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = VNPTResources.ID.Login_PhoneNotNull)]
        [RegularExpression(@"^(([0-9]*)|(([0-9]*)\.([0-9]*)))$", ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = VNPTResources.ID.MsgErrorRegularExpressionNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = VNPTResources.ID.Login_OTPNotNull)]
        [MaxLength(6, ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = VNPTResources.ID.Login_OTPOutOfSize)]
        [RegularExpression(@"^(([0-9]*)|(([0-9]*)\.([0-9]*)))$", ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = VNPTResources.ID.MsgErrorRegularExpressionNumber)]
        public string OTPCode { get; set; }
    }

    public class LogOutVM
    {
        [Required(ErrorMessageResourceType = typeof(Resources),
               ErrorMessageResourceName = VNPTResources.ID.Login_IDNotNull)]
        public Guid ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = VNPTResources.ID.Login_IDNotNull)]
        public Guid AccLogID { get; set; }
    }
}
