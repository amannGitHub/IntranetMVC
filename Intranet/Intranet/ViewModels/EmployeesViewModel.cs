using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class EmployeesViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("emp_id")]
        public int emp_id { get; set; }

        [Display(Name = "Employee")]
        public String EmployeeName { get; set; }

        [Display(Name = "Supervisor")]
        public String SupervisorName { get; set; }

        [Display(Name = "Ext")]
        public int? ext { get; set; }

        [Display(Name = "Email")]
        public String email { get; set; }

        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        public String Phone { get; set; }
        public String Location { get; set; }
    }
}

