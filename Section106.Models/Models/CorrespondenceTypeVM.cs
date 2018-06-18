using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Models.Models
{
    public class CorrespondenceTypeVM
    {
        public Int64 CorrespondenceTypeId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field Description must have maximum length of '50'.")]
        public string Description { get; set; }

        public string Template { get; set; }
    }
}
