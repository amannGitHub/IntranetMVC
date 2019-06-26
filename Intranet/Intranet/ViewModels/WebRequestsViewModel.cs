using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class WebRequestsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Request #")]
        public int WebRequestID { get; set; }

        [Display(Name = "Project #")]
        public string projnum { get; set; }

        [Display(Name = "Requestor")]
        public string EmployeeName { get; set; }

        [Display(Name = "Request Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? RequestDate { get; set; }

        [Display(Name = "Work Type")]
        public string WorkType { get; set; }

        [Display(Name = "Developer")]
        public string DeveloperName { get; set; }

        [Display(Name = "Status")]
        public string WebRequestStatus { get; set; }

        [Display(Name = "Max Hours")]
        public int MaxHours { get; set; }

        [Display(Name = "Assigned Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? AssignedDate { get; set; }

        [Display(Name = "Completed Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedDate { get; set; }
    }
}

