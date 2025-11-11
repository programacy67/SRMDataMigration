namespace SRMDataMigrationIgnite.Models
{
    using System;

    public class DMTransform
    {
        public int ID { get; set; }
        public string TransformType { get; set; }
        public bool? IsTextbox { get; set; }
        public string? TransformOptions { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool IsArchive { get; set; }
    }
}
