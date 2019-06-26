using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class RequestAssignee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssigneeID { get; set; }

        [Display(Name = "Assignee")]
        [Required(ErrorMessage = "Assignee is required")]
        public String EmployeeName { get; set; }
        
    }
}

