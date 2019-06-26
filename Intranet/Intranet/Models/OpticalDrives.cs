using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class OpticalDrives
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OpticalDriveID { get; set; }

        [Display(Name = "CD/DVD")]
        public string OpticalDrive { get; set; }

    }
}

