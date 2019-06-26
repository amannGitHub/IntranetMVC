using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class WebRequestsStatuses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WebRequestStatusID { get; set; }
        
        [Display(Name = "Status")]
        public string WebRequestStatus { get; set; }

    }
}

