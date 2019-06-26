using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class AlternateContacts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlternateContactID { get; set; }

        [Display(Name = "Alternate Contact")]
        [Required(ErrorMessage = "Alternate Contact is required")]
        public String AlternateContactName { get; set; }
        
    }
}

