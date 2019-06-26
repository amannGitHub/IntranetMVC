using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class EquipmentRequestStatuses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentRequestStatusID { get; set; }

        [Display(Name = "Status")]
        public string EquipmentRequestStatus { get; set; }
    }
}

