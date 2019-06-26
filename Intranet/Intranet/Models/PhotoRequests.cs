using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class PhotoRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? PhotoRequestID { get; set; }

        //[Display(Name = "Contract #")]
        //[Required(ErrorMessage = "Contract # is required")]
        //public string ContractNumber { get; set; }
        //^ might already be a part of the ProjectID, must check and fix this accordingly

        [Display(Name = "Project Name")]
        [Required(ErrorMessage = "Project Name is required")]
        public string ProjectName { get; set; }

        [Display(Name = "Project #")]
        [Required(ErrorMessage = "Project Code is required")]
        public int project_id { get; set; }

        [Display(Name = "Project Code")]
        public string ProjectCode { get; set; }

        //[Display(Name = "Task #")]
        //[Required(ErrorMessage = "Task Number is required")]
        //public string TaskNumber { get; set; }
        //^ might already be a part of ProjectID, must check and fix accordingly

        [Display(Name = "Request Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime? RequestDate { get; set; }

        [Display(Name = "Client")]
        [Required(ErrorMessage = "Selecting a Client is required")]
        public int? ClientID { get; set; }

        [Display(Name = "Division")]
        [Required(ErrorMessage = "Selecting a Division is required")]
        public int? DivisionID { get; set; }

        [Display(Name = "Location")]
        [DataType(DataType.MultilineText)]
        public string Location { get; set; }

        [Display(Name = "Picture #")]
        [DataType(DataType.MultilineText)]
        public string PictureNumber { get; set; }

        [Display(Name = "Requestor")]
        public int emp_id { get; set; }

        [Display(Name = "Status")]
        public int PhotoRequestStatusID { get; set; }

        //public virtual ICollection<Clients> Clients { get; set; }

        //public virtual ICollection<Divisions> Divisions { get; set; }
    }
}

