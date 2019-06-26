using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class GraphicsRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Ticket #")]
        public int? GraphicsRequestID { get; set; }

        [Display(Name = "Employee")]
        public int emp_id { get; set; }

        [Display(Name = "Project Code")]
        [Required(ErrorMessage = "Project Code is required")]
        public int project_id { get; set; }

        [Display(Name = "Project Title")]
        [Required(ErrorMessage = "Project Title are required")]
        public string ProjectTitle { get; set; }

        [Display(Name = "Notes/Special Instructions")]
        public string Instructions { get; set; }

        [Display(Name = "Draft Due")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime DraftDueDate { get; set; }

        [Display(Name = "Final Due")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime FinalDueDate { get; set; }

        [Display(Name = "Designer")]
        public int? AssigneeID { get; set; }

        [Display(Name = "Budgeted Hours")]
        [Required(ErrorMessage = "Budgeted Hours is required")]
        public int? MaxHours { get; set; }

        [Display(Name = "Additional Hours")]
        public int? AdditionalHours { get; set; }

        [Display(Name = "Request Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? RequestDate { get; set; }

        [Display(Name = "Assign Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? AssignDate { get; set; }

        [Display(Name = "Status Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime? StatusDate { get; set; }

        [Display(Name = "Status")]
        public int GraphicRequestStatusID { get; set; }

        [Display(Name = "Cover/Spine design")]
        public bool CoverSpineDesign { get; set; }

        [Display(Name = "Quantity:")]
        public int? CoverQuantity { get; set; }

        [Display(Name = "Size:")]
        public string NotebookSize { get; set; }

        [Display(Name = "Folder label")]
        public bool FolderLabel { get; set; }

        [Display(Name = "Quantity:")]
        public int? FolderLabelQuantity { get; set; }

        public bool Map { get; set; }

        [Display(Name = "Meeting poster")]
        public bool MeetingPoster { get; set; }

        [Display(Name = "Quantity:")]
        [Column("MeetingPosterQuantity")]
        public int? PosterQuantity { get; set; }

        [Display(Name = "Arrows:")]
        [Column("MeetingPosterArrows")]
        public int? PosterArrows { get; set; }

        [Display(Name = "Attachments:")]
        [Column("MeetingPosterAttachments")]
        public string PosterAttachments { get; set; }

        [Display(Name = "Name badges")]
        public bool NameBadges { get; set; }

        [Display(Name = "Poster session materials")]
        public bool PosterSessionMaterials { get; set; }

        [Display(Name = "PowerPoint Slide")]
        public bool PowerpointSlide { get; set; }

        [Display(Name = "Save-the-date card or postcard")]
        public bool SaveTheDatePostCard { get; set; }

        [Display(Name = "Table tents")]
        public bool TableTents { get; set; }

        public bool Tabs { get; set; }

        [Display(Name = "Web banner")]
        public bool WebBanner { get; set; }

        [Display(Name = "px size")]
        public string WebBannerSize { get; set; }

        [Display(Name = "Web rotator image")]
        public bool WebRotatorImage { get; set; }

        [Display(Name = "px size")]
        public string WebRotatorImageSize { get; set; }

        [Display(Name = "Other")]
        public bool ConferenceMaterialsOther { get; set; }

        public string ConferenceMaterialsOtherText { get; set; }

        public bool Brochures { get; set; }

        public bool FactSheets { get; set; }

        public bool Newsletter { get; set; }

        public bool Flyers { get; set; }

        [Display(Name = "Other:")]
        public bool BookPublishingOther { get; set; }

        public string BookPublishingOtherText { get; set; }

        [Display(Name = "Project Description")]
        public bool BookPublishingProject { get; set; }

        [Display(Name = "Description:")]
        public string BookPublishingProjectDescription { get; set; }

        [Display(Name = "Advertisements")]
        [Column("Advertisements")]
        public bool OtherAdvert { get; set; }

        [Display(Name = "Booth/Table top Exhibit Design")]
        public bool BoothExhibitDesign { get; set; }

        [Display(Name = "CD-ROM cover/disc duplication")]
        public bool DiscDuplication { get; set; }

        [Display(Name = "Chart/Diagram/Figures")]
        public bool ChartDiagramFigures { get; set; }

        [Display(Name = "File conversion (PDF, EPS, TIFF, JPEG, etc")]
        public bool FileConversion { get; set; }

        public bool Illustration { get; set; }

        [Display(Name = "Letterhead/envelopes/business cards")]
        public bool Letterhead { get; set; }

        [Display(Name = "Logo design")]
        public bool LogoDesign { get; set; }

        public bool Postcards { get; set; }

        [Display(Name = "Powerpoint slides")]
        public bool OtherPowerPointSlides { get; set; }

        [Display(Name = "Estimated number of slides:")]
        public int? OtherPowerPointSlidesNum { get; set; }

        public bool Scanning { get; set; }

        [Display(Name = "Other")]
        public bool OtherOther { get; set; }

        [Display(Name = "Other:")]
        public string OtherOtherDescription { get; set; }

        [Display(Name = "Project Description")]
        public bool OtherProject { get; set; }

        public string OtherProjectDescription { get; set; }

        [Display(Name = "Prep to Print")]
        public bool PrepToPrint { get; set; }

        [Display(Name = "PDF to Print")]
        public bool PDFtoPrint { get; set; }

        [Display(Name = "Scan to Print")]
        public bool ScanToPrint { get; set; }

        [Display(Name = "Quantity")]
        public bool PrintProductionQuantity { get; set; }

        public int? PrintProductionQuantityNumber { get; set; }

        [Display(Name = "Size:")]
        public string PrintProductionSize { get; set; }

        [Display(Name = "Pages:")]
        public string PrintProductionPages { get; set; }

        public bool Binding { get; set; }

        public bool Staple { get; set; }

        public bool Tapebind { get; set; }

        public bool Coil { get; set; }

        public bool Saddlestitch { get; set; }

        public bool GBC { get; set; }

        [Display(Name = "Project Description")]
        public bool PrintProductionProjectDescription { get; set; }

        public string PrintProductionProjectDescriptionText { get; set; }

        [Display(Name = "Existing Files")]
        public bool PrintProductionExistingFiles { get; set; }

        public string PrintProductionExistingFilesText { get; set; }

        [Display(Name = "Project Location")]
        public string ProjectLocation { get; set; }

        [Display(Name = "Requestor Approved")]
        public bool RequestorApproved { get; set; }

        //If we decide to go with keeping the record with a Deleted bit instead, uncomment below
        //public bool Deleted { get; set; }

        public virtual ICollection<File> Files { get; set; }
    }
}

