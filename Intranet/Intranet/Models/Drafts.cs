using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class Drafts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Draft #")]
        public int DraftID { get; set; }

        [Display(Name = "Editor/Proofreader")]
        public int EditorID { get; set; }

        [Display(Name = "Editorial Reviewer")]
        public int EditorialReviewerID { get; set; }

        public int EditingRequestID { get; set; }

        [Display(Name = "Draft")]
        [Required(ErrorMessage = "Draft name is required")]
        public string DraftName { get; set; }

        [Display(Name = "Editor Hours")]
        [Required(ErrorMessage = "Editor Hours is required")]
        public Single EditorHours { get; set; }

        [Display(Name = "Reviewer Hours")]
        [Required(ErrorMessage = "Reviewer Hours is required")]
        public Single EditorialHours { get; set; }

        [Display(Name = "Assigned")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Assigned Date is required")]
        public DateTime? DraftAssignedDate { get; set; }

        [Display(Name = "Draft Completed")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? DraftCompletedDate { get; set; }

        [Display(Name = "Review Completed")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? EditorialReviewCompleted { get; set; }

        [Display(Name = "Final Draft?")]
        public bool FinalDraft { get; set; }

        [Display(Name = "Editing Completed?")]
        public bool EditingCompleted { get; set; }

        [Display(Name = "Review Completed?")]
        public bool ReviewCompleted { get; set; }

        public string Notes { get; set; }

    }
}

