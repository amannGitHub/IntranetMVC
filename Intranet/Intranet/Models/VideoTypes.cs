using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class VideoTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VideoTypeID { get; set; }

        [Display(Name = "Video Type")]
        public string VideoType { get; set; }
    }
}

