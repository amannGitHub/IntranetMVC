using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class States
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "State ID")]
        public int StateID { get; set; }

        public string State { get; set; }

        [Display(Name = "Abbreviation")]
        public string Abbr { get; set; }
    }
}

