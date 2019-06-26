using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class OperatingSystems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OperatingSystemID { get; set; }

        [Display(Name = "Operating System")]
        public string OperatingSystem { get; set; }
    }
}

