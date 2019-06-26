using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class ITRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Ticket #")]
        public int? ITRequestID { get; set; }

        [Display(Name = "Employee")]
        public int emp_id { get; set; }

        [Display(Name = "Request Type")]
        [Required(ErrorMessage = "Request Type is required")]
        public int ITRequestTypeID { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string RequestDescription { get; set; }

        [Display(Name = "Project Code")]
        [Required(ErrorMessage = "Project Code is required")]
        public int project_id { get; set; }

        [Display(Name = "Assignee")]
        public int AssigneeID { get; set; }

        [Display(Name = "Assign Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? AssignDate { get; set; }

        [Display(Name = "Status")]
        public int ITRequestStatusID { get; set; }

        [Display(Name = "Status Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime StatusDate { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "IT Dept. Comments")]
        public string ITComments { get; set; }

        public virtual ICollection<ITRequestTypes> ITRequestTypes { get; set; }

    }
}

