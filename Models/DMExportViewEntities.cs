namespace SRMDataMigrationIgnite.Models
{
    using System;
    using System.Collections.Generic;
    
    public class DMExportViewEntities
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool IsArchive { get; set; }
    }
}
