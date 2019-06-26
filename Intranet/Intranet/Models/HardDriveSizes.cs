using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class HardDriveSizes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HardDriveSizeID { get; set; }

        [Display(Name = "Hard Drive")]
        public string HardDriveSize { get; set; }

        public bool Deleted { get; set; }
    }
}

