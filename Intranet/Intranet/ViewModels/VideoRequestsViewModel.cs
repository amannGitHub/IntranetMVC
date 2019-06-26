using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class VideoRequestsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Request #")]
        public int VideoRequestID { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [Display(Name = "Video Type")]
        public string VideoType { get; set; }

        [Display(Name = "Video Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Project/Task #")]
        public string projnum { get; set; }

        [Display(Name = "Status")]
        public string VideoRequestStatus { get; set; }

        [Display(Name = "Status Date")]
        [DataType(DataType.DateTime)]
        public DateTime StatusDate { get; set; }

        [Display(Name = "Due")]
        [DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Request Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Description")]
        public string VideoDescription { get; set; }
    }
}

