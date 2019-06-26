using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class MeetingsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Meeting #")]
        public int? MeetingID { get; set; }

        [Display(Name = "Project #")]
        public String Project { get; set; }

        [Display(Name = "Meeting Title")]
        public string MeetingTitle { get; set; }

        [Display(Name = "Planner")]
        public string MeetingPlanner { get; set; }

        //[Display(Name = "Meeting Short Name")]
        //public string ShortName { get; set; }

        public string Client { get; set; }

        [Display(Name = "Start")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime MeetingStartDate { get; set; }

        [Display(Name = "End")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime MeetingEndDate { get; set; }

        [Display(Name = "Location")]
        public String Location { get; set; }

        [Display(Name = "Registration End")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime RegistrationEndDate { get; set; }

        //[Display(Name = "Created By")]
        //public int? emp_id { get; set; }

    }
}

