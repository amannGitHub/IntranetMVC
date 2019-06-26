using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class EditingRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Request #")]
        public int? EditingRequestID { get; set; }

        [Display(Name = "Employee")]
        public int? emp_id { get; set; }

        [Display(Name = "Alternate Contact")]
        [Required(ErrorMessage = "Alternate Contact is required")]
        public int? AlternateContactID { get; set; }

        [Display(Name = "Specific Instructions")]
        public string RequestDescription { get; set; }

        [Display(Name = "Rush Job?")]
        public bool RushJob { get; set; }

        [Display(Name = "Date Submitted")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Internal Due Date/Time")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime InternalDueDate { get; set; }

        [Display(Name = "Status Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime StatusDate { get; set; }

        [Display(Name = "Project/Task #")]
        [Required(ErrorMessage = "Project/Task # is required")]
        public int project_id { get; set; }

        [Display(Name = "Project Title")]
        [Required(ErrorMessage = "Project Title is required")]
        public string ProjectTitle { get; set; }

        [Display(Name = "Budgeted Hours")]
        public decimal BudgetedHours { get; set; }

        [Display(Name = "Style Guide")]
        [Required(ErrorMessage = "Style Guide is required")]
        public int StyleGuideID { get; set; }

        [Display(Name = "Service Requested")]
        [Required(ErrorMessage = "Service Requested is required")]
        public int ServiceRequestedID { get; set; }

        [Display(Name = "Assigned Editor")]
        public int EditorID { get; set; }

        [Display(Name = "Assign Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? AssignedDate { get; set; }

        //[Display(Name = "Editorial Reviewer")]
        //public int EditorialReviewedID { get; set; }

        //[Display(Name = "QA Reviewer")]
        //public int QAReviewerID { get; set; }

        [Display(Name = "QA Date Received")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? QADateReceived { get; set; }

        [Display(Name = "QA Date Completed")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? QADateCompleted { get; set; }

        [Display(Name = "Other Service")]
        public string OtherService { get; set; }

        [Display(Name = "Source Materials Provided?")]
        public bool SourceMaterialsProvided { get; set; }

        [Display(Name = "Attached?")]
        public bool SourceAttached { get; set; }

        [Display(Name = "Hard Copy?")]
        public bool SourceHardCopy { get; set; }

        [Display(Name = "Conference Dept Review?")]
        public bool CDRN { get; set; }

        [Display(Name = "Conference Dept Approved?")]
        public bool CDRNApproved { get; set; }

        [Display(Name = "Status")]
        public int EditingRequestStatusID { get; set; }

        [Display(Name = "QA/QC Reviewer")]
        public int? QAReviewerID { get; set; }

        [Display(Name = "QA/QC Status")]
        public int? QAStatusID { get; set; }

        public string Notes { get; set; }

        [Display(Name = "QA/QC Approved?")]
        public bool QAApproved { get; set; }

        [Display(Name = "Submitter Approved?")]
        public bool SubmitterApproved { get; set; }

        public virtual ICollection<File> Files { get; set; }

    }
}

