using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class EquipmentRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Request #")]
        public int? EquipmentRequestID { get; set; }

        [Display(Name = "Employee")]
        public int emp_id { get; set; }

        [Display(Name = "Project Code")]
        [Required(ErrorMessage = "Project Code is required")]
        public int project_id { get; set; }

        [Display(Name = "Status")]
        public int EquipmentRequestStatusID { get; set; }

        [Display(Name = "Submitted")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime RequestSubmittedDate { get; set; }

        [Display(Name = "Decision Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime? RequestDecisionDate { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime RequestStartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [Required(ErrorMessage = "End Date is required")]
        public DateTime RequestEndDate { get; set; }

        [Display(Name = "Returned Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime? DateReturned { get; set; }

        [Display(Name = "Extension Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime? ExtensionDate { get; set; }
    }
}

