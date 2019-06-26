using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class WebRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Web Request #")]
        public int WebRequestID { get; set; }

        [Display(Name = "Requestor ID")]
        public int emp_id { get; set; }

        [Display(Name = "Project #")]
        [Required(ErrorMessage = "Project is required")]
        public int project_id { get; set; }

        [Display(Name = "Website")]
        [Required(ErrorMessage = "Website is required")]
        public int website_id { get; set; }

        [Display(Name = "Work Type")]
        [Required(ErrorMessage = "Work Type is required")]
        public int WorkTypeID { get; set; }

        [Display(Name = "Status")]
        public int WebRequestStatusID { get; set; }

        [Display(Name = "Request Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? RequestDate { get; set; }

        [Display(Name = "Developer")]
        public int? AssigneeID { get; set; }

        [Required(ErrorMessage = "Comments are required")]
        public string Comments { get; set; }

        [Display(Name = "Max Hours")]
        [Required(ErrorMessage = "Max Hours are required")]
        public Single MaxHours { get; set; }

        [Display(Name = "Developer Hours")]
        public Single? DevHours { get; set; }

        [Display(Name = "Assigned Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? AssignedDate { get; set; }

        [Display(Name = "Completed Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "Requestor Approved")]
        public bool RequestorApproved { get; set; }

        public virtual ICollection<File> Files { get; set; }
    }
}

