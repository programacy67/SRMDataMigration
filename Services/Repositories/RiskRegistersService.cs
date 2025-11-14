using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using SRMDataMigrationIgnite.Data;
using SRMDataMigrationIgnite.Models;
using SRMDataMigrationIgnite.Services.Interfaces;
using SRMDataMigrationIgnite.Utils;
using System.Data;

namespace SRMDataMigrationIgnite.Services.Repositories
{
    public class RiskRegistersService : IRiskRegistersService
    {
        string tableRisk = DatabaseTables.Risk.GetDescription();
        string tableProject = DatabaseTables.Project.GetDescription();
        string tableRStatus = DatabaseTables.RiskStatus.GetDescription();
        string tableRConclusion = DatabaseTables.RiskConclusion.GetDescription();
        string tableRFunctionType = DatabaseTables.FunctionType.GetDescription();
        string tableRUser = DatabaseTables.Users.GetDescription();
        string tableRTreatmentType = DatabaseTables.TreatmentType.GetDescription();
        string tableRType = DatabaseTables.RiskType.GetDescription();
        string tableRPriority = DatabaseTables.RiskPriority.GetDescription();
        string tableRElements = DatabaseTables.RiskElements.GetDescription();
        string tableRCategory = DatabaseTables.RiskCategory.GetDescription();
        string tableRCategories = DatabaseTables.RiskCategories.GetDescription();
        string tableRSource = DatabaseTables.RiskSource.GetDescription();
        string tableRAction = DatabaseTables.RiskAction.GetDescription();
        string tableRControlMeasure = DatabaseTables.ControlMeasure.GetDescription();
        string tableRImpact = DatabaseTables.ConsequenceImpact.GetDescription();
        private readonly IRepository _repository;
        private readonly ILogger<RiskRegistersService> _logger;

        public RiskRegistersService(IRepository repository, ILogger<RiskRegistersService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DataTable> GetAllRiskRegisters(Guid projectId)
        {
            var dt = new DataTable();
            try
            {
                string sql = $@"SELECT {tableProject}.Title AS Project, {tableRisk}.UID AS RiskID, {tableRisk}.Title, 
                    {tableRisk}.EventDescription, FORMAT({tableRisk}.EventDate, 'd', 'en-gb') AS EventDate, FORMAT({tableRisk}.EndDate, 'd', 'en-gb') AS EndDate, 
                    FORMAT({tableRisk}.NextReviewDate, 'd', 'en-gb') AS NextReviewDate, {tableRisk}.IsIssue, {tableRisk}.IsOpportunity, {tableRisk}.IsLowPriority, 
                    {tableRisk}.IsPublicRisk, {tableRisk}.IsEventOccur, {tableRisk}.IsCustomerAware, {tableRisk}.IsEndDateLinkedToPhase, 
                    {tableRConclusion}.Title AS RiskConclusion, 
                    {tableRisk}.CustomerRiskIdentification, {tableRisk}.Notes, {tableRisk}.RiskIdentifier, {tableRisk}.AlternativeRiskIdentifier, {tableRisk}.EventTrigger, 
                    {tableRisk}.EventVelocity, {tableRisk}.EventPersistence,
                    {tableRStatus}.Title AS RiskStatus, 
                    {tableRisk}.ActualImpactCost, {tableRisk}.TreatmentCost, {tableRisk}.ClosureStatus, FORMAT({tableRisk}.DateRaised, 'd', 'en-gb') AS DateRaised,
                    FORMAT({tableRisk}.DateClosed, 'd', 'en-gb') AS DateClosed, 
                    {tableRFunctionType}.Title AS FunctionType,
                    Own.UserName AS Owner, 
                    FORMAT({tableRisk}.ApprovedOn, 'd', 'en-gb') AS ApprovedOn, 
                    Approve.UserName AS ApprovedBy,
                    {tableRisk}.WBS, {tableRisk}.StrategyDescription, 
                    StrategyOwner.UserName AS StrategyOwner,
                    {tableRTreatmentType}.Title AS TreatmentType, 
                    {tableRisk}.FallbackStrategy, FORMAT({tableRisk}.StrategyPlannedStartDate, 'd', 'en-gb') AS StrategyPlannedStartDate, 
                    FORMAT({tableRisk}.StrategyActualStartDate, 'd', 'en-gb') AS StrategyActualStartDate, 
                    FORMAT({tableRisk}.StrategyPlannedEndDate, 'd', 'en-gb') AS StrategyPlannedEndDate, 
                    FORMAT({tableRisk}.StrategyActualEndDate, 'd', 'en-gb') AS StrategyActualEndDate, {tableRisk}.StrategyTriggerDescription, 
                    {tableRisk}.StrategyFallbackTriggerDescription, {tableRisk}.IsManagementRisk, {tableRisk}.RiskTrigger,  
                    {tableRisk}.ScoreResidual, {tableRisk}.ResidualRating, {tableRisk}.ScoreResidualLast, {tableRisk}.ImpactType, 
                    {tableRType}.Title AS RiskType,
                    {tableRPriority}.Title AS RiskPriority,
                    {tableRElements}.Title AS RiskElement,
                    {tableRisk}.InherentToleranceID, {tableRisk}.ResidualToleranceID, {tableRisk}.ResidualLikelihoodID, {tableRisk}.RiskBreakDownID,

                    (SELECT str({tableRAction}.UID) + ';' FROM {tableRAction} 
                    WHERE {tableRisk}.ID = {tableRAction}.RiskID FOR XML PATH('')) AS TempActionID, 
                    (SELECT {tableRAction}.Title + ';' FROM {tableRAction} 
                    WHERE {tableRisk}.ID = {tableRAction}.RiskID FOR XML PATH('')) AS ActionTitle, 

                    (SELECT str({tableRControlMeasure}.UID) + ';' FROM {tableRControlMeasure} 
                    WHERE {tableRisk}.ID = {tableRControlMeasure}.RiskID FOR XML PATH('')) AS ControlID, 
                    (SELECT {tableRControlMeasure}.Title + ';' FROM {tableRControlMeasure} 
                    WHERE {tableRisk}.ID = {tableRControlMeasure}.RiskID FOR XML PATH('')) AS ControlTitle, 

                    (SELECT str({tableRSource}.UID) + ';' FROM {tableRSource} 
                    WHERE {tableRisk}.ID = {tableRSource}.RiskID FOR XML PATH('')) AS TempSourceID, 
                    (SELECT {tableRSource}.Title + ';' FROM {tableRSource} 
                    WHERE {tableRisk}.ID = {tableRSource}.RiskID FOR XML PATH('')) AS SourceTitle, 

                    (SELECT str({tableRImpact}.UID) + ';' FROM {tableRImpact} 
                    WHERE {tableRisk}.ID = {tableRImpact}.RiskID FOR XML PATH('')) AS TempConsequenceId, 
                    (SELECT {tableRImpact}.Title + ';' FROM {tableRImpact} 
                    WHERE {tableRisk}.ID = {tableRImpact}.RiskID FOR XML PATH('')) AS ConsequenceTitle 

                    FROM {tableRisk} 
                    INNER JOIN {tableProject} ON {tableRisk}.ProjectID = {tableProject}.ID  
                    LEFT JOIN {tableRStatus} ON {tableRisk}.RiskStatusID = {tableRStatus}.ID 
                    LEFT JOIN {tableRConclusion} ON {tableRisk}.RiskConclusionID = {tableRConclusion}.ID 
                    LEFT JOIN {tableRFunctionType} ON {tableRisk}.FunctionTypeID = {tableRFunctionType}.ID 
                    LEFT JOIN {tableRUser} Own ON {tableRisk}.OwnerID = Own.ID 
                    LEFT JOIN {tableRUser} Approve ON {tableRisk}.ApprovedBy = Approve.ID 
                    LEFT JOIN {tableRUser} StrategyOwner ON {tableRisk}.StrategyOwnerID = StrategyOwner.ID 
                    LEFT JOIN {tableRTreatmentType} ON {tableRisk}.TreatmentTypeID = {tableRTreatmentType}.ID 
                    LEFT JOIN {tableRType} ON {tableRisk}.RiskTypeID = {tableRType}.ID 
                    LEFT JOIN {tableRPriority} ON {tableRisk}.RiskPriorityID = {tableRPriority}.ID 
                    LEFT JOIN {tableRElements} ON {tableRisk}.RiskElementID = {tableRElements}.ID 
                    WHERE {tableProject}.ID = N'{projectId.ToString()}'";

                //{tableProject}.UID AS ProjectID, (SELECT { tableRCategory}.Title + ';' FROM { tableRCategory}
                //INNER JOIN { tableRCategories}
                //on { tableRCategories}.RiskCategoryID = { tableRCategory}.ID
                //WHERE { tableRisk}.ID = { tableRCategories}.RiskID FOR XML PATH('')) AS Category,

                dt = await _repository.LoadDataTableAsync(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError("RiskRegisterService: " + ex.Message);
                throw new Exception("Exception: " + ex.Message);
            }
            return dt;
        }
    }
}
