using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class ProjectsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int project_id { get; set; }

        [Display(Name = "Contract #")]
        public string ContractNumber { get; set; }

        [Display(Name = "Project")]
        [Column("task_title")]
        public String ProjectName { get; set; }

        [Display(Name = "Project #")]
        [Column("projnum")]
        public String ProjectCode { get; set; }

        //[Display(Name = "Task #")]
        //[Column("tasknum")]
        //public String TaskNumber { get; set; }

        [Display(Name = "Client")]
        public String Client { get; set; }

        [Display(Name = "Division")]
        public String Division { get; set; }

        [Display(Name = "Project Title")]
        public String ProjectTitle { get; set; }

    }
}

