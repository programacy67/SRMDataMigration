using System.ComponentModel;

namespace SRMDataMigrationIgnite.Models
{
    public enum DatabaseTables
    {
        [Description("DMColumns")]
        DMColumns,

        [Description("DMExportViewEntities")]
        DMExportViewEntities,

        [Description("DMExportViewEntityColumns")]
        DMExportViewEntityColumns,

        [Description("DMTransform")]
        DMTransform,

        [Description("DMImport")]
        DMImport,

        [Description("DMImportConfig")]
        DMImportConfig,

        [Description("Project")]
        Project,

        [Description("Risk")]
        Risk,

        [Description("RiskConclusion")]
        RiskConclusion,

        [Description("RiskStatus")]
        RiskStatus,

        [Description("FunctionType")]
        FunctionType,

        [Description("Users")]
        Users,

        [Description("TreatmentType")]
        TreatmentType,

        [Description("RiskType")]
        RiskType,

        [Description("RiskPriority")]
        RiskPriority,

        [Description("RiskElements")]
        RiskElements,

        [Description("RiskCategory")]
        RiskCategory,

        [Description("RiskCategories")]
        RiskCategories,

        [Description("RiskAction")]
        RiskAction,

        [Description("ControlMeasure")]
        ControlMeasure,

        [Description("RiskSource")]
        RiskSource,

        [Description("ConsequenceImpact")]
        ConsequenceImpact,
    }
}
