using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class QAStaff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QAReviewerID { get; set; }

        [Required(ErrorMessage = "Editor is required")]
        public String QAReviewer { get; set; }
        
    }
}

