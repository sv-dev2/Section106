using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Models.Enums
{
    public enum RequestStatus
    {
        Saved = 1, // Saved but not submitted
        Submitted = 2, // submitted by user

        Returned = 3, // Returned by Admin 
        Assigned = 4, // Accepted by Admin and assigned to reviewers

        Completed = 5, // Approved by Admin
        Rejected = 6 // Rejected by Admin
    }
}
