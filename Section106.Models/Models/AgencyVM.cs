using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Models.Models
{
    public class AgencyVM
    {
        public Int64? AgencyId { get; set; }

        [Display(Name = "Agency Type")]
        public Int64 AgencyTypeId { get; set; }
        public string AgencyTypeName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field Name must have maximum length of '50'.")]
        public string Name { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "The field Number must have maximum length of '50'.")]
        public string Number { get; set; }

        public List<DictionaryVM> AgencyTypeList { get; set; }
    }
}
