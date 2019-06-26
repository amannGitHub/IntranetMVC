using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class LeaveRequestsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("leaverequest_id")]
        [Display(Name = "Request #")]
        public int leaverequest_id { get; set; }

        [Display(Name = "Employee ID")]
        public int emp_id { get; set; }

        [Display(Name = "Leave Type")]
        public string LeaveType { get; set; }

        [Display(Name = "EmployeeName")]
        public string EmployeeName { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        public DateTime? leave_start_dt { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        public DateTime? leave_end_dt { get; set; }      

        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Status Date")]
        [DataType(DataType.DateTime)]
        public DateTime? status_dt { get; set; }

        [Display(Name = "Request Date")]
        [DataType(DataType.DateTime)]
        public DateTime? create_date { get; set; }
    }
}

