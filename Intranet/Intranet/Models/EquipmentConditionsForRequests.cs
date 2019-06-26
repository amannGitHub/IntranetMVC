using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class EquipmentConditionsForRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentConditionID { get; set; }

        [Display(Name = "Condition")]
        public string EquipmentCondition { get; set; }
    }
}

