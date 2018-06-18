using System;
using DevExpress.Xpo;

namespace Section106.Models.Models
{

    public class CoverSheetVM
    {
        public string ProjectLogNumber { get; set; }
        public DateTime? Date { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string County { get; set; }
        public string LeadLegacy { get; set; }
        public string ApplicantName { get; set; }
        public ContactVM ApplicantContact { get; set; }
        public AgencyVM AgencyName { get; set; }
    }
        
}