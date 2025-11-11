namespace SRMDataMigrationIgnite.Models
{
    public class DMImportConfig
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string FieldMappings { get; set; }
        public string FieldTransforms { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool IsArchive { get; set; }
    }
}
