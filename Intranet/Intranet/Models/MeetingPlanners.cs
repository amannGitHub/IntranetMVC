using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class MeetingPlanners
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Column("emp_id")]
        public int MeetingPlannerID { get; set; }

        [Display(Name = "Meeting Planner")]
        [Required(ErrorMessage = "Meeting Planner is required")]
        public String MeetingPlanner { get; set; }

    }
}

