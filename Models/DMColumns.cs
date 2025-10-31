namespace SRMDataMigrationIgnite.Models
{
    using System;
    using System.Collections.Generic;
    
    public class DMColumns
    {
        public int ID { get; set; }
        public string CategoryTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public int ColumnPosition { get; set; }
        public bool IsMandatory { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool IsArchive { get; set; }
    }
}
