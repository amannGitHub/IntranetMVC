using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class PhotoRequestsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Request #")]
        public int PhotoRequestID { get; set; }

        [Display(Name = "Client")]
        public string Client { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Project #")]
        public string ProjectNumber { get; set; }

        [Display(Name = "Division")]
        public string Division { get; set; }

        [Display(Name = "Project Code")]
        public string ProjectCode { get; set; }

        [Display(Name = "Date")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Requestor")]
        public string EmployeeName { get; set; }

        [Display(Name = "Status")]
        public string PhotoRequestStatus { get; set; }

        //public string Location { get; set; }

        //public int? emp_id { get; set; }
    }
}

