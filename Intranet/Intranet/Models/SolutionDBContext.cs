using System.Data.Entity;

namespace Intranet.Models
{
    public class SolutionDBContext : DbContext
    {
        static SolutionDBContext()
        {
            Database.SetInitializer<SolutionDBContext>(null);
        }

        public SolutionDBContext()
            : base("Name=SolutionDBContext")
        {
        }

        public DbSet<EquipmentConditionsForRequests> EquipmentCondition { get; set; }
        public DbSet<EquipmentRequestDetails> EquipmentRequestDetail { get; set; }
        public DbSet<MeetingTracker> Meetings { get; set; }
        public DbSet<Inventories> Inventory { get; set; }
        public DbSet<EditingRequests> EditingRequest { get; set; }
        public DbSet<EditingRequestStatuses> EditingRequestStatus { get; set; }
        public DbSet<Drafts> Draft { get; set; }
        public DbSet<Employees> Employee { get; set; }
        public DbSet<LeaveRequests> LeaveRequest { get; set; }
        public DbSet<LeaveTypes> LeaveType { get; set; }
        public DbSet<EquipmentCategories> EquipmentCategories { get; set; }
        public DbSet<EquipmentCategoriesForRequests> EquipmentCategoriesForRequest { get; set; }
        public DbSet<EquipmentRequests> EquipmentRequest { get; set; }
        public DbSet<EquipmentForRequests> EquipmentForRequest { get; set; }
        public DbSet<EquipmentRequestStatuses> EquipmentRequestStatus { get; set; }
        public DbSet<GraphicsRequests> GraphicRequest { get; set; }
        public DbSet<GraphicRequestStatuses> GraphicRequestStatus { get; set; }
        public DbSet<VideoRequests> VideoRequest { get; set; }
        public DbSet<VideoRequestStatuses> VideoRequestsStatus { get; set; }
        public DbSet<VideoFileTypes> VideoFileType { get; set; }
        public DbSet<VideoTypes> VideoType { get; set; }
        public DbSet<ITRequests> ITRequest { get; set; }
        public DbSet<ITRequestStatuses> ITRequestsStatus { get; set; }
        public DbSet<ITRequestTypes> ITRequestsType { get; set; }
        public DbSet<Projects> Project { get; set; }
        public DbSet<WebRequests> WebRequest { get; set; }
        public DbSet<WebsiteNames> WebsiteName { get; set; }
        public DbSet<WorkTypes> WorkType { get; set; }
        public DbSet<Vendors> Vendor { get; set; }
        public DbSet<Holidays> Holiday { get; set; }
        public DbSet<Clients> Client { get; set; }
        public DbSet<Divisions> Division { get; set; }
        public DbSet<WebRequestsStatuses> WebRequestsStatus { get; set; }
        public DbSet<File> File { get; set; }
        public DbSet<ErrorModel> ErrorModel { get; set; }
        public DbSet<States> States { get; set; }
        public DbSet<StyleGuides> StyleGuide { get; set; }
        public DbSet<ServicesRequested> ServicesRequested { get; set; }
        public DbSet<HardDriveSizes> HardDriveSize { get; set; }
        public DbSet<EquipmentTypes> EquipmentType { get; set; }
        public DbSet<CPUs> CPU { get; set; }
        public DbSet<Manufacturers> Manufacturer { get; set; }
        public DbSet<EquipmentModels> EquipmentModel { get; set; }
        public DbSet<RAMs> RAM { get; set; }
        public DbSet<OperatingSystems> OperatingSystem { get; set; }
        public DbSet<OpticalDrives> OpticalDrive { get; set; }
        public DbSet<Toners> Toners { get; set; }
        public DbSet<PhotoRequests> PhotoRequests { get; set; }
        public DbSet<Locations> Locations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EquipmentConditionsForRequests>().ToTable("tblEquipmentConditions");
            modelBuilder.Entity<EquipmentRequestDetails>().ToTable("tblEquipmentRequestDetails");
            modelBuilder.Entity<MeetingTracker>().ToTable("tblMeetingTracker");
            modelBuilder.Entity<Inventories>().ToTable("tblInventory");
            modelBuilder.Entity<EditingRequests>().ToTable("tblEditingRequests");
            modelBuilder.Entity<EditingRequestStatuses>().ToTable("tblEditingRequestStatuses");
            modelBuilder.Entity<Drafts>().ToTable("tblDrafts");
            modelBuilder.Entity<Employees>().ToTable("tblEmployees");
            modelBuilder.Entity<LeaveRequests>().ToTable("tblLeaveRequest");
            modelBuilder.Entity<LeaveTypes>().ToTable("tblLeaveLeaveTypes");
            modelBuilder.Entity<EquipmentCategoriesForRequests>().ToTable("tblEquipmentCategoriesForRequests");
            modelBuilder.Entity<EquipmentCategories>().ToTable("tblEquipmentCategories");
            modelBuilder.Entity<EquipmentRequests>().ToTable("tblEquipmentRequests");
            modelBuilder.Entity<EquipmentForRequests>().ToTable("tblEquipmentForRequests");
            modelBuilder.Entity<EquipmentRequestStatuses>().ToTable("tblEquipmentRequestStatuses");
            modelBuilder.Entity<GraphicsRequests>().ToTable("tblGraphicsRequests");
            modelBuilder.Entity<GraphicRequestStatuses>().ToTable("tblGraphicRequestStatuses");
            modelBuilder.Entity<VideoRequests>().ToTable("tblVideoRequests");
            modelBuilder.Entity<VideoRequestStatuses>().ToTable("tblVideoRequestsStatus");
            modelBuilder.Entity<VideoFileTypes>().ToTable("tblVideoFileTypes");
            modelBuilder.Entity<VideoTypes>().ToTable("tblVideoTypes");
            modelBuilder.Entity<ITRequests>().ToTable("tblITRequests");
            modelBuilder.Entity<ITRequestStatuses>().ToTable("tblITRequestsStatus");
            modelBuilder.Entity<ITRequestTypes>().ToTable("tblITRequestTypes");
            modelBuilder.Entity<Projects>().ToTable("tblProjects");
            modelBuilder.Entity<WebRequests>().ToTable("tblWebRequests");
            modelBuilder.Entity<WebsiteNames>().ToTable("tblWebsites");
            modelBuilder.Entity<WorkTypes>().ToTable("tblWorkTypes");
            modelBuilder.Entity<Vendors>().ToTable("tblVendors");
            modelBuilder.Entity<Holidays>().ToTable("tblHolidays");
            modelBuilder.Entity<Clients>().ToTable("tblClients");
            modelBuilder.Entity<Divisions>().ToTable("tblDivisions");
            modelBuilder.Entity<WebRequestsStatuses>().ToTable("tblWebRequestStatuses");
            modelBuilder.Entity<File>().ToTable("tblFiles");
            modelBuilder.Entity<ErrorModel>().ToTable("tblErrors");
            modelBuilder.Entity<States>().ToTable("tblStates");
            modelBuilder.Entity<StyleGuides>().ToTable("tblStyleGuides");
            modelBuilder.Entity<ServicesRequested>().ToTable("tblServicesRequested");
            modelBuilder.Entity<Toners>().ToTable("tblToners");
            modelBuilder.Entity<HardDriveSizes>().ToTable("tblHardDriveSizes");
            modelBuilder.Entity<EquipmentTypes>().ToTable("tblEquipmentTypes");
            modelBuilder.Entity<CPUs>().ToTable("tblCPUs");
            modelBuilder.Entity<Manufacturers>().ToTable("tblManufacturers");
            modelBuilder.Entity<EquipmentModels>().ToTable("tblEquipmentModels");
            modelBuilder.Entity<RAMs>().ToTable("tblRAMs");
            modelBuilder.Entity<OperatingSystems>().ToTable("tblOperatingSystems");
            modelBuilder.Entity<OpticalDrives>().ToTable("tblOpticalDrives");
            modelBuilder.Entity<PhotoRequests>().ToTable("tblPhotoRequests");
            modelBuilder.Entity<Locations>().ToTable("tblMeetingLocations");
        }
    }
}
