using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class DraftsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Draft #")]
        public int DraftID { get; set; }

        [Display(Name = "Editor/Proofreader")]
        public string Editor { get; set; }

        [Display(Name = "Editorial Reviewer")]
        public string Reviewer { get; set; }

        public int EditingRequestID { get; set; }

        [Display(Name = "Draft")]
        public string DraftName { get; set; }

        [Display(Name = "Editor Hours")]
        public Single EditorHours { get; set; }

        [Display(Name = "Reviewer Hours")]
        public Single EditorialHours { get; set; }

        [Display(Name = "Assigned")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime DraftAssignedDate { get; set; }

        [Display(Name = "Completed")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? DraftCompletedDate { get; set; }

        [Display(Name = "Review Completed")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? EditorialReviewCompleted { get; set; }

        [Display(Name = "Final Draft?")]
        public bool FinalDraft { get; set; }

    }
}

