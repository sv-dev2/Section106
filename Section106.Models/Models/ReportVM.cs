using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Models.Models
{
    public class ReportVM
    {
        public long Id { get; set; }
        public int? EligibleProperties { get; set; }
        public int? InEligibleProperties { get; set; }
        public int? Unknown { get; set; }
        public string Request { get; set; }
    }
}
