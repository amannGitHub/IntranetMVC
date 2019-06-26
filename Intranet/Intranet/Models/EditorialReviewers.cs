using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class EditorialReviewers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EditorialReviewerID { get; set; }

        [Required(ErrorMessage = "Editor is required")]
        public String EditorialReviewer { get; set; }
        
    }
}

