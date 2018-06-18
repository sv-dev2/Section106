using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Section106.Models.Models
{
    public class CorrespondenceVM
    {
        public Int64? CorrespondenceId { get; set; }
        public Int64 CorrespondenceTypeId { get; set; }
        public string CorrespondenceTypeName { get; set; }
        [Required]
        [AllowHtml]
        public string Body { get; set; }

        public string PlainBody { get; set; }
        public string Date { get; set; }
    }
}
