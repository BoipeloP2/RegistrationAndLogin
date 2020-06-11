using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RegistrationAndLogin.Models
{
    public class UserLogin
    {
        [Display(Name ="Email Address")]
        [Required(AllowEmptyStrings = false,ErrorMessage ="Email Id Required")]
        public string EmailId { get; set; }


       // [Display(Name ="Password")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Password Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; }

    }
}