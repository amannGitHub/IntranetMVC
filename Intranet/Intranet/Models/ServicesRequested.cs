using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class ServicesRequested
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Service Requested ID")]
        public int ServiceRequestedID { get; set; }

        [Display(Name = "Service Requested")]
        public string ServiceRequested { get; set; }
    }
}

