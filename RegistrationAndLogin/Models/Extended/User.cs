using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RegistrationAndLogin.Models
{

    [MetadataType(typeof(UserMetaData))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }

    }

    public  class UserMetaData
    {
        [Display(Name = "Fisrt Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Error!  Fisrt Name is Rquired")]
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Error! Last Name is Rquired")]
        public string LastName { get; set; }

        [Display(Name = "Email ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Error! Email Id is Rquired")]
        [DataType(DataType.EmailAddress)]
        public string EmailId { get; set; }


        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date )]
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString ="{0:MM/ddyyyy}")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Error! Password is Required")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Minimum of 6 characters is required")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Confirm Password and Password do not match")]
        public string ConfirmPassword{ get; set; }


    }
}