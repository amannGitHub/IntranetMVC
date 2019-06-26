using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class File
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileID { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public byte[] ContentFile { get; set; }
        public FileTypes FileType { get; set; }

        public int? WebRequestID { get; set; }
        public int? EditingRequestID { get; set; }
        public int? GraphicsRequestID { get; set; }

        public virtual WebRequests WebRequest { get; set; }

        public virtual EditingRequests EditingRequest { get; set; }

        public virtual GraphicsRequests GraphicRequest { get; set; }
    }
}