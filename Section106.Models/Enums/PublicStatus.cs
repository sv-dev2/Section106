using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Models.Enums
{
    public enum PublicStatus
    {
        Saved = 1,    //saved by user
        Submitted = 2,  // Submitted by User
        Returned = 3,   // Returned by Admin 
        Hold = 4,       // More info need by Archaeologist, Architecture ,Technical
        Completed = 5,   // Approved by Admin
        Rejected = 6,   // Rejected by Admin
        [Display(Name = "Under Architecture Review")]
        UnderArchitectureReview = 7,
        [Display(Name = "Under Archaeology Review")]
        UnderArchaeologyReview = 8,       
        [Display(Name = "Under Technical Review")]
        UnderTechnicalReview = 9,
        Final = 10,      // All Reviews are done, Admin Approval pending
    }
}
