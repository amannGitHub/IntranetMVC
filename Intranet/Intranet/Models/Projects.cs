using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class Projects
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int project_id { get; set; }

        [Display(Name = "Contract #")]
        [Required(ErrorMessage = "Contract # is required")]
        public string ContractNumber { get; set; }

        [Display(Name = "Project Title")]
        [Column("task_title")]
        [Required(ErrorMessage = "Project Name is required")]
        public String ProjectName { get; set; }

        [Display(Name = "Project #")]
        [Column("projnum")]
        [Required(ErrorMessage = "Project # is required")]
        public String ProjectCode { get; set; }

        [Display(Name = "Task #")]
        [Column("tasknum")]
        [Required(ErrorMessage = "Task Number is required")]
        public String TaskNumber { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        //[Required(ErrorMessage = "Start Date is required")]
        [Column("proj_start_date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        //[Required(ErrorMessage = "End Date is required")]
        [Column("proj_end_date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        //[Required(ErrorMessage = "Start Date is required")]
        [Column("create_date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Client")]
        //[Required(ErrorMessage = "Client is required")]
        public int? ClientID { get; set; }

        [Display(Name = "Division")]
        //[Required(ErrorMessage = "Division is required")]
        public int? DivisionID { get; set; }

        [Display(Name = "Expired")]
        public bool Expired { get; set; }

        //public virtual ICollection<Clients> Clients { get; set; }

        //public virtual ICollection<Divisions> Divisions { get; set; }
    }
}

