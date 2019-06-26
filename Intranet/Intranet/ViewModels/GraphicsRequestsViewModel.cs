using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class GraphicsRequestsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Request #")]
        public int GraphicsRequestID { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [Display(Name = "Project Title")]
        public string ProjectTitle { get; set; }

        [Display(Name = "Designer")]
        public string AssigneeName { get; set; }

        [Display(Name = "Draft Due Date")]
        [DataType(DataType.DateTime)]
        public DateTime DraftDueDate { get; set; }

        [Display(Name = "Status")]
        public string GraphicsRequestStatus { get; set; }

        [Display(Name = "Final Due Date")]
        [DataType(DataType.DateTime)]
        public DateTime FinalDueDate { get; set; }

        [Display(Name = "Request Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Instructions")]
        public string Instructions { get; set; }
    }
}

