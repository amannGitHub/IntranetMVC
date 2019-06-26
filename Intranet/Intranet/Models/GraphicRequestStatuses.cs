using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class GraphicRequestStatuses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GraphicRequestStatusID { get; set; }

        [Display(Name = "Status")]
        public string GraphicRequestStatus { get; set; }
    }
}

