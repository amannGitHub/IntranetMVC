using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class InventoryViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Inventory #")]
        public int InventoryID { get; set; }

        [Display(Name = "Employee")]
        public String Employee { get; set; }

        [Display(Name = "Model")]
        public String EquipmentModel { get; set; }

        [Display(Name = "Make")]
        public String Manufacturer { get; set; }

        [Display(Name = "Type")]
        public String EquipmentType { get; set; }

        [Display(Name = "Category")]
        public String EquipmentCategory { get; set; }

    }
}

