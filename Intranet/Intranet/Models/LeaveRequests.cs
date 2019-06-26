using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class LeaveRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("leaverequest_id")]
        public int leaverequest_id { get; set; }

        [Display(Name = "Employee ID")]
        public int emp_id { get; set; }

        [Display(Name = "Leave Type")]
        [Required(ErrorMessage = "Leave Type is required")]
        public int leavetype_id { get; set; }

        [Display(Name = "Leave Reason")]
        [Required(ErrorMessage = "Leave Reason is required")]
        public string leave_reason { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime leave_start_dt { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [Required(ErrorMessage = "End Date is required")]
        public DateTime leave_end_dt { get; set; }

        [Display(Name = "Total Hours")]
        [Required(ErrorMessage = "Total Hours is required")]
        public Single totalhours { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Status Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime status_dt { get; set; }

        [Display(Name = "Request Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? create_date { get; set; }

        [Display(Name = "Manager Description")]
        [DataType(DataType.MultilineText)]
        public string OtherComments { get; set; }

        [Display(Name = "HR Approved")]
        public bool? HRApproved { get; set; }

        [Display(Name = "HR Set Sick")]
        public Single? HRSickHours { get; set; }

        [Display(Name = "HR Set Vacation")]
        public Single? HRVacationHours { get; set; }

        //[Display(Name = "Requestor")]
        //public string EmployeeName { get; set; }
    }
}

