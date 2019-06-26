using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class VideoFileTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VideoFileTypeID { get; set; }

        [Display(Name = "File Type")]
        public string VideoFileType { get; set; }

        //public int SortOrder { get; set; }
    }
}

