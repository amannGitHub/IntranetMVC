using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class ITRequestsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Request #")]
        public int ITRequestID { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [Display(Name = "Request Type")]
        public string ITRequestType { get; set; }

        [Display(Name = "Description")]
        public string RequestDescription { get; set; }

        [Display(Name = "Project Code")]
        public string ProjectCode { get; set; }

        [Display(Name = "Assignee")]
        public string AssigneeName { get; set; }

        [Display(Name = "Assign Date")]
        [DataType(DataType.DateTime)]
        public DateTime AssignDate { get; set; }

        [Display(Name = "Status")]
        public string ITRequestStatus { get; set; }

        [Display(Name = "Status Date")]
        [DataType(DataType.DateTime)]
        public DateTime StatusDate { get; set; }

        [Display(Name = "Request Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "IT Comments")]
        public string ITComments { get; set; }

    }
}

