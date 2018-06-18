using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Models.Models
{
    public class ContactVM
    {
        public Int64? Id { get; set; }

        public string UserId { get; set; }

        [Display(Name = "User Name")]
        [MaxLength(256, ErrorMessage = "The field User Name must have maximum length of '256'.")]
        public string UserName { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "The field Name must have maximum length of '200'.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Company")]
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

        [Display(Name = "State")]
        public string StateName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field City must have maximum length of '50'.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [MaxLength(5, ErrorMessage = "Zip must be of 5 digits")]
        [MinLength(5, ErrorMessage = "Zip must be of 5 digits")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Zip must be numeric")]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [MaxLength(14, ErrorMessage = "Fax must be of 10 digits maximum")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Fax must be numeric")]
        [Display(Name = "Fax")]
        public string Fax { get; set; }

       
        [Display(Name = "Mobile Number")]
        [MaxLength(14, ErrorMessage = "Mobile Number must be of 10 digits")]
        [MinLength(14, ErrorMessage = "Mobile Number must be of 10 digits")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Phone must be numeric")]
        public string MobileNumber { get; set; }
        [Required]
        [Display(Name = "Office Number")]
        [MaxLength(14, ErrorMessage = "Office Number must be of 10 digits")]
        [MinLength(14, ErrorMessage = "Office Number must be of 10 digits")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Phone must be numeric")]
        public string OfficeNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [MaxLength(256, ErrorMessage = "The field Email must have maximum length of '256'.")]
        public string Email { get; set; }

        public string Role { get; set; }

        public List<DictionaryVM> States { get; set; }
        public List<DictionaryVM> Cities { get; set; }
        public List<DictionaryVM> Counties { get; set; }
        [Display(Name = "County")]
        public int? CountyId { get; set; }
        [Display(Name = "County")]
        public string CountyName { get; set; }
    }
}
