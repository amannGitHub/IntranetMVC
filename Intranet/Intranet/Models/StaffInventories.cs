using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class StaffInventories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Inventory #")]
        public int InventoryID { get; set; }

        [Required(ErrorMessage = "Employee is required")]
        [Display(Name = "Employee")]
        public int emp_id { get; set; }

        [Display(Name = "HD Size")]
        public int HardDriveID { get; set; }

        [Display(Name = "CPU")]
        public int CPUID { get; set; }

        [Display(Name = "Operating System")]
        public int OperatingSystemID { get; set; }

        [Display(Name = "RAM")]
        public int RAMID { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [Display(Name = "Model")]
        public int EquipmentModelID { get; set; }

        [Required(ErrorMessage = "Make is required")]
        [Display(Name = "Make")]
        public int ManufacturerID { get; set; }

        [Display(Name = "Type")]
        public int EquipmentTypeID { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int EquipmentCategoryID { get; set; }

        [Required(ErrorMessage = "Serial Number is required")]
        [Display(Name = "Serial Number")]
        public string Serial { get; set; }

        public string Comments { get; set; }

        public string Location { get; set; }

        [Display(Name = "Created Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Deleted?")]
        public bool Deleted { get; set; }
    }
}

