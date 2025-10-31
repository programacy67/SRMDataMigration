namespace SRMDataMigrationIgnite.Models
{
    using System;
    using System.Collections.Generic;
    
    public class DMExportViewEntityColumns
    {
        public Guid ID { get; set; }
        public Guid DMExportViewEntityID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool IsArchive { get; set; }
    }
}
