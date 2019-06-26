using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class PhotoRequestStatuses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PhotoRequestStatusID { get; set; }

        [Display(Name = "Status")]
        public string PhotoRequestStatus { get; set; }
    }
}

