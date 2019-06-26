using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class Employees
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("emp_id")]
        public int emp_id { get; set; }

        public int supervisor_id { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        public String emp_fname { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        public String emp_lname { get; set; }

        [Display(Name = "Network Name")]
        [Required(ErrorMessage = "Network Name is required")]
        public String network_id { get; set; }

        [Display(Name = "Sick Hours")]
        public Single? sickhours { get; set; }

        [Display(Name = "Sick Hours")]
        public Single? sickrate { get; set; }

        [Display(Name = "Vacation Hours")]
        public Single? vacationhours { get; set; }

        [Display(Name = "Vacation Hours")]
        public Single? vacationrate { get; set; }

        [Display(Name = "Ext")]
        public int? ext { get; set; }

        [Display(Name = "Email")]
        public String email { get; set; }
    }
}

