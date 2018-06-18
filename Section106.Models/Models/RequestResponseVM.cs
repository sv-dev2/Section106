using Section106.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Models.Models
{
    public class RequestResponseVM
    {
        public Int64? RequestResponseId { get; set; }
        public Int64 RequestAssignmentId { get; set; }
        public Int64 RequestId { get; set; }
        public ReviewerResponseType ResponseType { get; set; }
        public bool IsHistoricProperty { get; set; }
        public bool IsNoHistoricProperty { get; set; }
        //public bool IsNoEffect { get; set; }
        //public bool IsNoAdverseEffect { get; set; }
        //public bool IsNoAdverseEffectWithCondition { get; set; }

        [Display(Name = "Architect Response")]
        public short ArchitectResponse { get; set; }

        [Display(Name = "Architect Comment")]
        [MaxLength(500, ErrorMessage = "The field Architect Comment must have maximum length of '500'.")]
        public string ArchitectComment { get; set; }
        public string ArchitectUserId { get; set; }



        public bool IsSurveyRequired { get; set; }
        public bool IsMoreInfoRequired { get; set; }
        public bool IsArchaeologyNoHistoricProperty { get; set; }
        public bool IsArchaeologyHistoricProperty { get; set; }

        public bool IsArchaeologyNoEffect { get; set; }
        public bool IsArchaeologyNoAdverseEffect { get; set; }
        public bool IsArchaeologyAdverseEffect { get; set; }

        [Display(Name = "Archaelogical Response")]
        public short ArchaelogicalResponse { get; set; }

        [Display(Name = "Archaelogical Comment")]
        [MaxLength(500, ErrorMessage = "The field Archaelogical Comment must have maximum length of '500'.")]
        public string ArchaelogicalComment { get; set; }
        public string ArchaelogicalUserId { get; set; }




        public bool IsTechnicalNoEffect { get; set; }
        public bool IsTechnicalNoAdverseEffect { get; set; }
        public bool IsTechnicalAdverseEffect { get; set; }

        [Display(Name = "Technical Response")]
        public short TechnicalResponse { get; set; }

        [Display(Name = "Technical Comment")]
        [MaxLength(500, ErrorMessage = "The field Technical Comment must have maximum length of '500'.")]
        public string TechnicalComment { get; set; }
        public string TechnicalUserId { get; set; }
        public int? EligibleProperties { get; set; }
        public int? InEligibleProperties { get; set; }
        public int? UnknownProperties { get; set; }

        [Display(Name = "Landmarks Response")]
        public short LandMarksResponse { get; set; }

        [Display(Name = "Landmarks Comment")]
        [MaxLength(500, ErrorMessage = "The field Landmarks Comment must have maximum length of '500'.")]
        public string LandmarksComment { get; set; }
        public string LandmarksUserId { get; set; }

    }
}
