using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class EquipmentCategoriesForRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentCategoryID { get; set; }

        [Display(Name = "Equipment Category")]
        public string EquipmentCategory { get; set; }
    }
}

