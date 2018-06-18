using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Section106.Models.Models
{
    public class RequestVM
    {

        public bool IsProjectCompleted { get; set; }
        public Int64? RequestId { get; set; }
        public int Status { get; set; }
        public int AssignmentStatus { get; set; }
        public int AssignmentSubStatus { get; set; }
        public string StatusStr { get; set; }
        public string UserId { get; set; }

        public string UserName { get; set; }


        //Project Related Properties
        public bool IsNewSubmission { get; set; }

        [Display(Name = "Project Name")]
        [MaxLength(50, ErrorMessage = "The field Project Name must have maximum length of '50'.")]
        [Required]
        public string ProjectName { get; set; }

        [Required]
        [Display(Name = "Brief Project Description")]
        [MaxLength(500, ErrorMessage = "The field Brief Project Description must have maximum length of '500'.")]
        public string ProjectDescription { get; set; }

        [Display(Name = "Project Log Number")]
        public string ProjectLogNumber { get; set; }
        [Display(Name = "Date")]

        [Required]
        public DateTime? ProjectDate { get; set; }

        [Display(Name = "Date")]
        public string ProjectDateStr { get; set; }

        //[Required]
        [Display(Name = "Address 1")]
        [MaxLength(200, ErrorMessage = "The field Address 1 must have maximum length of '200'.")]
        public string ProjectAddress1 { get; set; }


        [Display(Name = "Address 2")]
        [MaxLength(200, ErrorMessage = "The field Address 2 must have maximum length of '200'.")]
        public string ProjectAddress2 { get; set; }
        [Required]
        [Display(Name = "County")]
        public int? ProjectCountyId { get; set; }
        [Display(Name = "City")]
        //[Required]
        public int? ProjectCityId { get; set; }

        [MaxLength(5, ErrorMessage = "Zip must be of 5 digits")]
        [MinLength(5, ErrorMessage = "Zip must be of 5 digits")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Zip must be numeric")]
        [Display(Name = "Zip")]
        public string ProjectZip { get; set; }

        //[RegularExpression(@"^-{1,3}\d*\.{0,1}\d+$", ErrorMessage = "Invalid Latitude")]
        //[RegularExpression("^([1-9]{2,3})[.]{1}[0-9]{2,9}$", ErrorMessage = "Latitude must be in '00 or 000[.]00 to 000000000' format.")]
        [DisplayFormat(DataFormatString = "{0:0.000000000}", ApplyFormatInEditMode = true)]
        public decimal? Latitude { get; set; }

        //[RegularExpression("^-\\d+$([1-9]{2,3})[.]{1}[0-9]{2,9}$", ErrorMessage = "Longitute must be in '00 or 000[.]00 to 000000000' format.")]
        [DisplayFormat(DataFormatString = "{0:0.000000000}", ApplyFormatInEditMode = true)]
        public decimal? Longitude { get; set; }

        [MaxLength(50, ErrorMessage = "The field TownShip must have maximum length of '50'.")]
        [Display(Name = "Township")]
        public string TownShip { get; set; }

        [MaxLength(50, ErrorMessage = "The field Range must have maximum length of '50'.")]
        public string Range { get; set; }

        [MaxLength(50, ErrorMessage = "The field Section must have maximum length of '50'.")]
        public string Section { get; set; }
        public List<DictionaryVM> Counties { get; set; }
        public List<DictionaryVM> Cities { get; set; }
        public List<DictionaryVM> States { get; set; }
        public List<DictionaryVM> Agencies { get; set; }




        //Request Related Properties
        public bool IsFederalProperty { get; set; }

        public bool IsStateProperty { get; set; }

        public bool IsMunicipalProperty { get; set; }

        public bool IsPrivateProperty { get; set; }

        [Required]
        [Display(Name = "Lead Agency")]
        public Int64 AgencyId { get; set; }

        public string AgencyName { get; set; }


        [Display(Name = "Lead Agency Project Number")]
        [MaxLength(50, ErrorMessage = "The field Lead Agency Project Number must have maximum length of '50'.")]
        public string AgencyProjectNumber { get; set; }
        //public string OtherLeadAgency { get; set; }
        public bool IsConstructionExcavation { get; set; }
        public bool IsRehabilitation { get; set; }
        public bool IsDemolition { get; set; }
        public bool IsSaleOrTransfer { get; set; }
        public bool IsNonConstructionLoan { get; set; }

        public bool IsOther { get; set; }

        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "The field Description must have maximum length of '50'.")]
        public string OtherDescription { get; set; }


        [Display(Name = "Project Area (Total acres)")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Project area must be numeric")]
        [Range(1, 2147483647, ErrorMessage = "Value must be between 1 to 2147483647")]
        public int? TotalProjectArea { get; set; }

        [Display(Name = "Ground Disturbance (Total acres)")]
        [Range(1, 2147483647, ErrorMessage = "Value must be between 1 to 2147483647")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Ground disturbance must be numeric")]
        public int? TotalGroundDisturbance { get; set; }

        [Display(Name = "Check if buildings or structures in the project area are 50 years or older")]
        public bool IsOldStructure { get; set; }

        [Display(Name = "Does this project involve properties listed in or eligible for the National Register of Historic Places, or designated by a local government?")]
        public Int16 IsHistoricOrGovtProperty { get; set; }

        [Display(Name = "If 'Yes', Name of the historic property or historic districts, if known")]
        [MaxLength(50, ErrorMessage = "The field Historic Property Name must have maximum length of '50'.")]
        public string HistoricPropertyName { get; set; }

        public bool IsDocumentSentViaEmail { get; set; }

        public bool? IsRequestAssignedAlready { get; set; }
        public string RemainingDays { get; set; }
        public string InternalRemainingDays { get; set; }
        [Display(Name = "Does this project include properties that are designated Mississippi Landmarks?")]
        public bool IsMississippiLandmarks { get; set; }

        //Submitting Contact Properties
        public ContactVM SubmittingContact { get; set; }




        //Applicant Contact Properties
        public long? ApplicantContactId { get; set; }
        public bool IsApplicantSameAsSubmitting { get; set; }
        public ContactVM ApplicantContact { get; set; }

        public List<AttachmentVM> Attachments { get; set; }
        public string AttachmentList { get; set; }


        public Int64? CorrespondenceTypeId { get; set; }
        public List<CorrespondenceVM> CorrespondenceList { get; set; }
        public List<DictionaryVM> CorrespondenceTypes { get; set; }
        public RequestAssignmentVM RequestAssignment { get; set; }
        public RequestResponseVM RequestResponse { get; set; }

    }
}
