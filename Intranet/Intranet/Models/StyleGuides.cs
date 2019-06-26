using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class StyleGuides
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Style Guide ID")]
        public int StyleGuideID { get; set; }

        [Display(Name = "Style Guide")]
        public string StyleGuide { get; set; }
    }
}

