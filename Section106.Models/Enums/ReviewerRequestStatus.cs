using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Models.Enums
{
    public enum ReviewerRequestStatus
    {
        Pending = 1, // Is in progress
        Approved = 2, // Approved by Reviewers
        Rejected = 3, // Rejected by Reviewers
        MoreInfoRequired = 4, // Reviewer need more information about request
        SurveyRequired = 5 // Survey required for request review
    }
}
