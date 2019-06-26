using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class EditingRequestStatuses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EditingRequestStatusID { get; set; }

        [Display(Name = "Status")]
        public string EditingRequestStatus { get; set; }
    }
}

