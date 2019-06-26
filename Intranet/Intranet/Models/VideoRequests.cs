using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class VideoRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Request #")]
        public int? VideoRequestID { get; set; }

        [Display(Name = "Video Project Name")]
        [Required(ErrorMessage = "Project Name is required")]
        public String ProjectName { get; set; }

        [Display(Name = "Employee")]
        public int emp_id { get; set; }

        [Display(Name = "Project/Task #")]
        [Required(ErrorMessage = "Project is required")]
        public int project_id { get; set; }

        [Display(Name = "Due Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Due Date is required")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string VideoDescription { get; set; }

        [Display(Name = "Hosting Location")]
        public string HostingLocation { get; set; }

        [Display(Name = "Presentation Location")]
        public string PresentationLocation { get; set; }

        [Display(Name = "Other Files Location")]
        public string OtherFilesLocation { get; set; }

        [Display(Name = "Video Type")]
        [Required(ErrorMessage = "Video Type is required")]
        public int VideoTypeID { get; set; }

        [Display(Name = "File Type")]
        [Required(ErrorMessage = "File Type is required")]
        public int VideoFileTypeID { get; set; }

        [Display(Name = "Status")]
        public int VideoRequestStatusID { get; set; }

        [Display(Name = "Status Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime StatusDate { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Requestor Approved")]
        public bool RequestorApproved { get; set; }
    }
}

