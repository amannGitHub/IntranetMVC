using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class Editors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EditorID { get; set; }

        [Required(ErrorMessage = "Editor is required")]
        public String Editor { get; set; }
        
    }
}

