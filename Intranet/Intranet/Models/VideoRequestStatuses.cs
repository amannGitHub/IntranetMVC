using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class VideoRequestStatuses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VideoRequestStatusID { get; set; }

        [Display(Name = "Status")]
        public string VideoRequestStatus { get; set; }
    }
}

