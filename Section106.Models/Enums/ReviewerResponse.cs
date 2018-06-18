using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Models.Enums
{
    public enum ReviewerResponse
    {
        Eligible = 1,
        NotEligible = 2,
        RequestAdditionalInformation = 3,
        SurveyRequired = 4
    }
}
