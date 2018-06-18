using Section106.Models.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Section106.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        //[EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public long? Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [MaxLength(256, ErrorMessage = "The field Email must have maximum length of '256'.")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[RegularExpression("^[a-z][A-Z][0-9]*$", ErrorMessage = "The password must contain atleast one capital letter, one special character and one numeric value")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "User Name")]
        [MaxLength(256, ErrorMessage = "The field User Name must have maximum length of '256'.")]
        public string UserName { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "The field Name must have maximum length of '200'.")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field Company must have maximum length of '50'.")]
        public string Company { get; set; }

        [Required]
        [Display(Name = "Address 1")]
        [MaxLength(200, ErrorMessage = "The field Address 1 must have maximum length of '200'.")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        [MaxLength(200, ErrorMessage = "The field Address 2 must have maximum length of '200'.")]
        public string Address2 { get; set; }

        [Display(Name = "State")]
        public int StateId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field City must have maximum length of '50'.")]
        public string City { get; set; }

        [Required]
        [MaxLength(5, ErrorMessage = "Zip must be of 5 digits")]
        [MinLength(5, ErrorMessage = "Zip must be of 5 digits")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Zip must be numeric")]
        public string Zip { get; set; }

        [MaxLength(14, ErrorMessage = "Fax must be of 10 digits maximum")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Fax must be numeric")]
        public string Fax { get; set; }

        [Required]
        [Display(Name = "Mobile Phone")]
        [MaxLength(14, ErrorMessage = "Mobile Phone must be of 10 digits")]
        [MinLength(14, ErrorMessage = "Mobile Phone must be of 10 digits")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Phone must be numeric")]
        public string MobilePhone { get; set; }

        [Display(Name = "Office Phone")]
        [MaxLength(14, ErrorMessage = "Office Phone must be of 10 digits")]
        [MinLength(14, ErrorMessage = "Office Phone must be of 10 digits")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Office Phone must be numeric")]
        public string OfficePhone { get; set; }
        public List<DictionaryVM> States { get; set; }
        public List<DictionaryVM> Cities { get; set; }
        public List<DictionaryVM> Counties { get; set; }
        public string Role { get; set; }
        public List<RoleVM> ReviewersRoleList { get; set; }
        [Display(Name = "Contacted By")]

        [Required]
        public int ContactedBy { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
