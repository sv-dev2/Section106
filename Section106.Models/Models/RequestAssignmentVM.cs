using Section106.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Models.Models
{
    public class RequestAssignmentVM
    {
        public Int64? RequestAssignmentId { get; set; }
        public Int64 RequestId { get; set; }
        public bool IsAccept { get; set; }
        public short? FederalOrState { get; set; }

        [Display(Name = "Project Number")]
        [MaxLength(50, ErrorMessage = "The field Project Number must have maximum length of '50'.")]
        public string ProjectNumber { get; set; }

        [Display(Name = "Respond Date")]
        public DateTime? RespondDate { get; set; }

        [Display(Name = "Respond Date")]
        public string RespondDateStr { get; set; }
        public bool IsAssignToArchitect { get; set; }
        public bool IsAssignToArchaelogical { get; set; }
        public bool IsAssignToTechnical { get; set; }
        public bool IsAssignToLandMarks { get; set; }
        public ReviewerRequestStatus ArchitechStatus { get; set; }
        public ReviewerRequestStatus ArchaelogicalStatus { get; set; }
        public ReviewerRequestStatus TechnicalStatus { get; set; }
        public ReviewerRequestStatus LandMarksStatus { get; set; }
    }
}
