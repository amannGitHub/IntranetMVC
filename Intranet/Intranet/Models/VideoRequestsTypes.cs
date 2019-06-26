using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class VideoRequestTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VideoRequestTypeID { get; set; }

        [Display(Name = "Request Type")]
        public string VideoRequestType { get; set; }

        public virtual VideoRequests VideoRequest { get; set; }
    }
}

