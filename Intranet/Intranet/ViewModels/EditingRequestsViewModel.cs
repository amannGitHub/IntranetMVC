using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class EditingRequestsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Request #")]
        public int? EditingRequestID { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [Display(Name = "Project #")]
        public String Project { get; set; }

        [Display(Name = "Project Title")]
        public String ProjectTitle { get; set; }

        [Display(Name = "Assignee")]
        public int AssigneeID { get; set; }

        [Display(Name = "Employee")]
        public int emp_id { get; set; }

        [Display(Name = "Service Requested")]
        public String ServiceRequested { get; set; }

        [Display(Name = "Style Guide")]
        public String StyleGuide { get; set; }

        [Display(Name = "Status")]
        public String EditingRequestStatus { get; set; }

        //[Display(Name = "Assign Date")]
        //[DisplayFormat(DataFormatString = "{0:g}")]
        //[DataType(DataType.DateTime)]
        //public DateTime? AssignDate { get; set; }

        //[Display(Name = "Status")]
        //public String EditorRequestStatus { get; set; }

        [Display(Name = "Request Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Internal Due Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime InternalDueDate { get; set; }

        //[Display(Name = "Status Date")]
        //[DisplayFormat(DataFormatString = "{0:g}")]
        //[DataType(DataType.DateTime)]
        //public DateTime StatusDate { get; set; }

        public bool RushJob { get; set; }

    }
}

