using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class EquipmentForRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Equipment #")]
        public int? EquipmentID { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Item category is required")]
        public int EquipmentCategoryID { get; set; }  

        [Display(Name = "Condition")]
        [Required(ErrorMessage = "Item condition is required")]
        public int? EquipmentConditionID { get; set; }

        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        public string Notes { get; set; }

        public bool Deleted { get; set; }

    }
}

