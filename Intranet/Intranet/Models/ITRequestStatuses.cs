using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class ITRequestStatuses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ITRequestStatusID { get; set; }

        [Display(Name = "Status")]
        public string ITRequestStatus { get; set; }
    }
}

