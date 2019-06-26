using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class LeaveBalance
    {
        [Display(Name = "AccruedVacationHours")]
        public Decimal AccruedVacationHours { get; set; }

        [Display(Name = "VacationHours")]
        public Decimal VacationHours { get; set; }

        [Display(Name = "VacationRate")]
        public Decimal VacationRate { get; set; }

        [Display(Name = "Sick Hours")]
        public Decimal SickHours { get; set; }

        [Display(Name = "AccruedSickHours")]
        public Decimal AccruedSickHours { get; set; }

        [Display(Name = "SickRate")]
        public Decimal SickRate { get; set; }
    }
}

