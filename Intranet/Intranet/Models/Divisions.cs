using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class Divisions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Division #")]
        public int DivisionID { get; set; }

        [Required(ErrorMessage = "Division is required")]
        public string Division { get; set; }

        [Display(Name = "Acronym")]
        [Required(ErrorMessage = "Acronym is required")]
        public string Acronym { get; set; }

        public bool Deleted { get; set; }
    }
}

