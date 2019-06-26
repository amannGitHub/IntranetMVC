using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.ViewModels
{
    public class EquipmentForRequestsViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Equipment #")]
        public int EquipmentID { get; set; }

        [Display(Name = "Category")]
        public string EquipmentCategory { get; set; }

        //Check and see what label we want
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Display(Name = "Item Description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Item Condition")]
        public string EquipmentCondition { get; set; }

        [Display(Name = "Deleted?")]
        public bool Deleted { get; set; }

        //this is only used by the Equipmentment Requested partial view, not the Equipment For Requests index
        public int? EquipmentRequestDetailID { get; set; }
        
    }
}

