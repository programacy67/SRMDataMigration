using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SRMDataMigrationIgnite.Data;
using SRMDataMigrationIgnite.Models;
using SRMDataMigrationIgnite.Services.Interfaces;
using SRMDataMigrationIgnite.Utils;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.Json;
using static SRMDataMigrationIgnite.Services.Repositories.DMExportViewEntitiesService;

namespace SRMDataMigrationIgnite.Controllers
{
    public class ExportController : Controller
    {
        private readonly DataSanitizer _sanitizer;

        private readonly IRiskRegistersService _iRiskRegistersService;
        private readonly IDMExportViewEntitiesService _iDMExportViewEntitiesService;
        private readonly IDMExportViewEntityColumnsService _iDMExportViewEntityColumnsService;

        public ExportController(DataSanitizer sanitizer, IRiskRegistersService riskRegistersService, 
                IDMExportViewEntitiesService dmExportViewEntitiesService, IDMExportViewEntityColumnsService dmExportViewEntityColumnsService)
        {
            _sanitizer = sanitizer;
            _iRiskRegistersService = riskRegistersService;
            _iDMExportViewEntitiesService = dmExportViewEntitiesService;
            _iDMExportViewEntityColumnsService = dmExportViewEntityColumnsService;
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string? hfViewName)
        {
            string viewName = hfViewName;
            if (!string.IsNullOrEmpty(viewName))
            {
                Guid viewGuid = new Guid();
                if (viewName == viewGuid.ToString()) viewName = "Default";
            }
            
            if (string.IsNullOrEmpty(viewName)) viewName = "Default";

            try
            {   
                DataTable dt = await _iRiskRegistersService.GetAllRiskRegisters(ApplicationDbContext.projectId);
                List<ViewEntityCategoryData> dtCategory = await _iDMExportViewEntitiesService.GetRiskCategory(ApplicationDbContext.cancellationToken);

                //Get all Risk Identification
                List<ViewEntityCategoryData> dtIdentificationCategory = (from dtc in dtCategory where dtc.CategoryTitle.Equals("Risk Identification") select dtc).ToList();
                //DataRow[] filteredRows = dtCategory.Where(i => i.CategoryTitle.Equals("Risk Identification").ToList();
                //DataTable dtIdentificationCategory = filteredRows.Length > 0 ? filteredRows.CopyToDataTable() : new DataTable();

                //Get all Risk Source
                List<ViewEntityCategoryData> dtSourceCategory = (from dtc in dtCategory where dtc.CategoryTitle.Equals("Risk Source") select dtc).ToList();

                //Get all Risk Actions
                List<ViewEntityCategoryData> dtActionsCategory = (from dtc in dtCategory where dtc.CategoryTitle.Equals("Risk Actions") select dtc).ToList();

                //Get all Risk Controls
                List<ViewEntityCategoryData> dtControlsCategory = (from dtc in dtCategory where dtc.CategoryTitle.Equals("Risk Controls") select dtc).ToList();

                //Get all Risk Impact
                List<ViewEntityCategoryData> dtImpactCategory = (from dtc in dtCategory where dtc.CategoryTitle.Equals("Risk Impact") select dtc).ToList();

                //Get all saved columns if any for a particular UserId
                ViewData["ViewName"] = viewName;
                DataTable dtUserColumns = await _iDMExportViewEntitiesService.GetUserEntities(viewName, ApplicationDbContext.userId);

                //Display the saved columns in its order
                List<string?> columnsToShow = new List<string?>();
                List<string?> columnsToHide = new List<string?>();

                List<ViewEntityData> columnsToView = await _iDMExportViewEntitiesService.GetAllUserEntities(ApplicationDbContext.userId, ApplicationDbContext.cancellationToken);
                if (columnsToView?.Count > 0)
                    columnsToView.Add(new ViewEntityData
                    {
                        Title = "Default",
                        ID = new Guid()
                    });
                ViewData["ViewEntities"] = JsonSerializer.Serialize(columnsToView);

                if (string.IsNullOrEmpty(viewName) || viewName == "Default")
                {
                    //    columnsToShow = dtCategory.AsEnumerable()
                    //                         .Select(row => row.Field<string>("Title"))
                    //                         .Distinct().ToList();
                    //    var ColToKeep = _sanitizer.CleanDataTable(dtCategory, "");
                    //    ViewData["ColumnsToShow"] = JsonSerializer.Serialize(ColToKeep, _sanitizer.JsonOptions());

                    columnsToShow = (from dtc in dtCategory select dtc.Title).Distinct().ToList();
                    ViewData["ColumnsToShow"] = JsonSerializer.Serialize(columnsToShow);
                    ViewData["ColumnsToHide"] = JsonSerializer.Serialize(columnsToHide, _sanitizer.JsonOptions());
                }
                if (dtUserColumns != null)
                {
                    if (dtUserColumns.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(viewName) && viewName != "Default")
                        {
                            columnsToShow = dtUserColumns.AsEnumerable()
                                                .Select(row => row.Field<string>("Title"))
                                                .Distinct().ToList();
                            ViewData["ColumnsToShow"] = JsonSerializer.Serialize(columnsToShow);
                        }
                        
                        dt.SetColumnsOrder(columnsToShow.ToArray());
                        columnsToHide = (from dc in dt.Columns.Cast<DataColumn>() where !columnsToShow.Contains(dc.ColumnName) select (dc.ColumnName)).ToList();
                        ViewData["ColumnsToHide"] = JsonSerializer.Serialize(columnsToHide, _sanitizer.JsonOptions());
                    }
                }

                var columns = MiscellaneousService.GetColumnDefinitions(dt);
                var dataList = _sanitizer.CleanDataTable(dt, "");
                ViewData["Columns"] = JsonSerializer.Serialize(columns, _sanitizer.JsonOptions());
                ViewData["Data"] = JsonSerializer.Serialize(dataList, _sanitizer.JsonOptions());

                //var identificationCategory = dtIdentificationCategory.AsEnumerable().Select(row => new { Title = row["Title"], IsMandatory = row["IsMandatory"] }).ToList();
                ViewData["IdentificationCategory"] = dtIdentificationCategory;
                ViewData["SourceCategory"] = dtSourceCategory;
                ViewData["ActionsCategory"] = dtActionsCategory;
                ViewData["ControlsCategory"] = dtControlsCategory;
                ViewData["ImpactCategory"] = dtImpactCategory;
                return View(dt);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
        }
                
        //[HttpPost]
        //public IActionResult ExportToExcel([FromBody] List<string> selectedColumns)
        //{
        //    if (selectedColumns == null || !selectedColumns.Any())
        //        return BadRequest("No columns selected.");

        //    // 1. Build SQL SELECT statement
        //    Dictionary<int, string> columnName = new Dictionary<int, string>();
        //    Dictionary<int, string> columnNameDict = new Dictionary<int, string>();
        //    int i = 0;
        //    foreach (var col in selectedColumns)
        //    {
        //        columnNameDict.Add(i, col);
        //        i = i + 1;
        //    }
        //    string sqlColumns = string.Empty;
        //    if (columnNameDict.Values.Contains("ProjectName"))
        //        columnName.Add(columnNameDict.FirstOrDefault(x => x.Value == "ProjectName").Key, "Project.Title AS ProjectName");

        //    if (selectedColumns.Contains("UID"))
        //        columnName.Add(columnNameDict.FirstOrDefault(x => x.Value == "UID").Key, "Risk.UID");

        //    if (selectedColumns.Contains("Title"))
        //        columnName.Add(columnNameDict.FirstOrDefault(x => x.Value == "Title").Key, "Risk.Title");

        //    if (selectedColumns.Contains("EventDescription"))
        //        columnName.Add(columnNameDict.FirstOrDefault(x => x.Value == "EventDescription").Key, "Risk.EventDescription");

        //    if (selectedColumns.Contains("EventDate"))
        //        columnName.Add(columnNameDict.FirstOrDefault(x => x.Value == "EventDate").Key, "FORMAT(Risk.EventDate, 'd', 'en-gb') AS EventDate");

        //    if (selectedColumns.Contains("Notes"))
        //        columnName.Add(columnNameDict.FirstOrDefault(x => x.Value == "Notes").Key, "Risk.Notes");

        //    if (selectedColumns.Contains("RiskIdentifier"))
        //        columnName.Add(columnNameDict.FirstOrDefault(x => x.Value == "RiskIdentifier").Key, "Risk.RiskIdentifier");

        //    if (selectedColumns.Contains("AlternativeRiskIdentifier"))
        //        columnName.Add(columnNameDict.FirstOrDefault(x => x.Value == "AlternativeRiskIdentifier").Key, "Risk.AlternativeRiskIdentifier");

        //    if (selectedColumns.Contains("Status"))
        //        columnName.Add(columnNameDict.FirstOrDefault(x => x.Value == "Status").Key, "RiskStatus.Title AS Status");

        //    if (selectedColumns.Contains("Category"))
        //    {
        //        sqlColumns = "(SELECT RiskCategory.Title + ';' FROM RiskCategory " +
        //            " INNER JOIN RiskCategories on RiskCategories.RiskCategoryID = RiskCategory.ID " +
        //            " WHERE Risk.ID = RiskCategories. RiskID FOR XML PATH ('')) AS Category";
        //        columnName.Add(columnNameDict.FirstOrDefault(x => x.Value == "Category").Key, sqlColumns);
        //    }

        //    var columns = string.Join(",", columnName.OrderBy(kv => kv.Key).Select(col => $"{col.Value}"));

        //    string sql = string.Format("SELECT {0} FROM Risk " +
        //        "INNER JOIN Project ON Project.ID = Risk.ProjectID " +
        //        "LEFT JOIN RiskStatus ON Risk.RiskStatusID = RiskStatus.ID " +
        //        "WHERE Project.ID = N'{1}'", columns, ApplicationDbContext.projectId);

        //    // 2. Execute SQL and load into DataTable
        //    var dataTable = new DataTable();
        //    using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        //    using (var command = new SqlCommand(sql, connection))
        //    using (var adapter = new SqlDataAdapter(command))
        //    {
        //        connection.Open();
        //        adapter.Fill(dataTable);
        //    }

        //    // 3. Create Excel file
        //    using var workbook = new XLWorkbook();
        //    var worksheet = workbook.Worksheets.Add("Export");
        //    worksheet.Cell(1, 1).InsertTable(dataTable);

        //    using var stream = new MemoryStream();
        //    workbook.SaveAs(stream);
        //    stream.Position = 0;

        //    // 4. Return file
        //    return File(stream.ToArray(),
        //        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //        "Export.xlsx");
        //}

        [HttpPost]
        public async Task<IActionResult> SaveSettings([FromBody] UserRequest payload) 
        {
            var user = ApplicationDbContext.userId;
            Guid dmExportGuid = Guid.NewGuid();
            Guid dmExportEntityGuid = new Guid();
            DateTime dtNow = DateTime.UtcNow;
            string viewsList = string.Empty;

            DMExportViewEntities dmExport = new DMExportViewEntities();
            dmExport.ID = dmExportGuid;
            dmExport.UserID = user;
            dmExport.Title = payload.Viewname;
            dmExport.CreatedOn = dtNow;
            dmExport.CreatedBy = user;

            List<DMExportViewEntityColumns> dmExportColumnsList = new List<DMExportViewEntityColumns>();
            DMExportViewEntityColumns dmExportColumns = new DMExportViewEntityColumns();
            int i = 0;
            foreach (var str in payload.Columns)
            {
                dmExportEntityGuid = Guid.NewGuid();
                dmExportColumns = new DMExportViewEntityColumns();
                dmExportColumns.ID = dmExportEntityGuid;
                dmExportColumns.DMExportViewEntityID = dmExportGuid;
                dmExportColumns.Title = str;
                dmExportColumns.DisplayOrder = i;
                dmExportColumns.CreatedOn = dtNow;
                dmExportColumns.CreatedBy = user;

                i = i + 1;
                dmExportColumnsList.Add(dmExportColumns);
            }

            try
            {
                await _iDMExportViewEntitiesService.AddView(dmExport);
                await _iDMExportViewEntityColumnsService.AddEntityColumns(dmExportColumnsList);

                // Get all views for this user
                List<ViewEntityData> columnsToView = await _iDMExportViewEntitiesService.GetAllUserEntities(ApplicationDbContext.userId, ApplicationDbContext.cancellationToken);
                if (columnsToView?.Count > 0)
                    columnsToView.Add(new ViewEntityData { Title = "Default", ID = new Guid() });

                viewsList = JsonSerializer.Serialize(columnsToView);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok(new { message = "Saved", views = viewsList, newViewId = dmExportGuid.ToString() });
        }

        public async Task<IActionResult> EditSettings([FromBody] UserRequest payload)
        {
            var user = ApplicationDbContext.userId;
            Guid dmExportGuid = new Guid(payload.Viewname);
            Guid dmExportEntityGuid = new Guid();
            DateTime dtNow = DateTime.UtcNow;
            List<DMExportViewEntityColumns> dmExportColumnsList = new List<DMExportViewEntityColumns>();
            DMExportViewEntityColumns dmExportColumns = new DMExportViewEntityColumns();
            int i = 0;
            foreach (var str in payload.Columns)
            {
                dmExportEntityGuid = Guid.NewGuid();
                dmExportColumns = new DMExportViewEntityColumns();
                dmExportColumns.ID = dmExportEntityGuid;
                dmExportColumns.DMExportViewEntityID = dmExportGuid;
                dmExportColumns.Title = str;
                dmExportColumns.DisplayOrder = i;
                dmExportColumns.CreatedOn = dtNow;
                dmExportColumns.CreatedBy = user;

                i = i + 1;
                dmExportColumnsList.Add(dmExportColumns);
            }

            try
            {
                await _iDMExportViewEntityColumnsService.DeleteEntityColumns(dmExportGuid, user);
                await _iDMExportViewEntityColumnsService.AddEntityColumns(dmExportColumnsList);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok(new { message = "Saved" });
        }

        public async Task<IActionResult> DeleteSettings([FromBody] UserRequest payload)
        {
            var user = ApplicationDbContext.userId;
            DateTime dtNow = DateTime.UtcNow;
            string viewsList = string.Empty;

            Guid dmExportGuid = new Guid(payload.Viewname);
            DMExportViewEntities dmExport = await _iDMExportViewEntitiesService.GetViewEntities(dmExportGuid, ApplicationDbContext.cancellationToken);
            if (dmExport != null)
            {
                try
                {
                    dmExport.IsArchive = true;
                    dmExport.ModifiedOn = dtNow;
                    dmExport.ModifiedBy = user;
                    await _iDMExportViewEntitiesService.DeleteView(dmExport);

                    // Get all views for this user
                    List<ViewEntityData> columnsToView = await _iDMExportViewEntitiesService.GetAllUserEntities(ApplicationDbContext.userId, ApplicationDbContext.cancellationToken);
                    if (columnsToView?.Count > 0)
                        columnsToView.Add(new ViewEntityData { Title = "Default", ID = new Guid() });

                    viewsList = JsonSerializer.Serialize(columnsToView);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return Ok(new { message = "Deleted", views = viewsList });
        }

        // Data Models
        public class UserRequest
        {
            public string Viewname { get; set; }
            public List<string> Columns { get; set; }
        }
    }
}


