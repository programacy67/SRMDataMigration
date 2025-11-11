using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FuzzySharp;
// using FuzzyString;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SRMDataMigrationIgnite.Data;
using SRMDataMigrationIgnite.Models;
using SRMDataMigrationIgnite.Services.Interfaces;
using SRMDataMigrationIgnite.Services.Repositories;
using SRMDataMigrationIgnite.Utils;
using System.Data;
using System.Text.Json;
using static SRMDataMigrationIgnite.Services.Repositories.DMExportViewEntitiesService;

namespace SRMDataMigrationIgnite.Controllers
{
    public class ImportController : Controller
    {
        private readonly IRiskRegistersService _iRiskRegistersService;
        private readonly IDMExportViewEntitiesService _iDMExportViewEntitiesService;
        private readonly IDMImportConfig _iDMImportConfig;

        public ImportController(DataSanitizer sanitizer, IRiskRegistersService riskRegistersService, IDMExportViewEntitiesService dmExportViewEntitiesService,
            IDMImportConfig iDMImportConfig)
        {
            _iRiskRegistersService = riskRegistersService;
            _iDMExportViewEntitiesService = dmExportViewEntitiesService;
            _iDMImportConfig = iDMImportConfig;
        }

        public async Task<IActionResult> Index()
        {          
            List<ViewEntityCategoryData> dtCategory = await _iDMExportViewEntitiesService.GetRiskCategory(ApplicationDbContext.cancellationToken);
            List<string?> DbColumns = (from dtc in dtCategory select dtc.Title).Distinct().ToList();

            
            List<DMTransform> transformTypeList = await _iDMImportConfig.GetTransformList(ApplicationDbContext.cancellationToken);
            List<string?> transformTypes = (from tf in transformTypeList select tf.TransformType).Distinct().ToList();
            ViewData["DbTransform"] = JsonSerializer.Serialize(transformTypeList);
            ViewData["DbTransformType"] = JsonSerializer.Serialize(transformTypes);
            ViewData["DbColumns"] = JsonSerializer.Serialize(DbColumns);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            if (file != null && file.Length > (1024 * 1024 * 50)) // 50MB limit
            {
                return Json(new { success = false, message = "Your file is too large. Maximum size allowed is 50MB!" });
            }

            var result = new ExcelPreviewModel();
            string filePath = string.Empty;
            string path = "/Uploads";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            filePath = Path.Combine(Directory.GetCurrentDirectory(), path, file.FileName);
            string extension = Path.GetExtension(file.FileName);
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    foreach (var sheet in workbook.Worksheets)
                    {
                        result.WorksheetNames.Add(sheet.Name);
                        var preview = new WorksheetPreview
                        {
                            WorksheetName = sheet.Name
                        };

                        var range = sheet.RangeUsed();
                        if (range != null)
                        {
                            foreach (var row in range.RowsUsed().Take(5))
                            {
                                preview.Rows.Add(row.Cells().Select(c => c.GetValue<string>()).ToList());
                            }
                        }

                        result.Sheets.Add(preview);
                    }
                }
            }

            return Json(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> ColumnFuzzyMatch([FromBody] FuzzyRequest request)
        {
            List<ViewEntityCategoryData> dtCategory = await _iDMExportViewEntitiesService.GetRiskCategory(ApplicationDbContext.cancellationToken);
            List<string> DbColumns = (from dtc in dtCategory select dtc.Title).Distinct().ToList();

            //var comparisonOptions = new List<FuzzyStringComparisonOptions>
            //{
            //    FuzzyStringComparisonOptions.UseNormalizedLevenshteinDistance,
            //    FuzzyStringComparisonOptions.UseJaroWinklerDistance,
            //    FuzzyStringComparisonOptions.UseJaccardDistance
            //};

            int threshold = 70;
            var matchedCells = new List<object>();
            foreach (var cellValue in request.Headers)
            {
                // using FuzzySharp
                var matches = DbColumns
                .Select(col => new
                {
                    Column = col,
                    Score = Fuzz.PartialRatio(cellValue, col)
                })
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

                //    var matches = DbColumns
                //        .Select(dbCol => new
                //        {
                //            bool isSimilar = cellValue.ApproximatelyEquals(
                //                dbCol,
                //                comparisonOptions,
                //                FuzzyStringComparisonTolerance.Normal
                //            );
                //    int maxLen = Math.Max(cellValue.Length, dbCol.Length);
                //    double similarity = 1.0 - (double)cellValue.LevenshteinDistance(dbCol) / maxLen;

                //    return new
                //    {
                //        DbColumn = dbCol,
                //        Score = isSimilar ? similarity : similarity * 0.5 // reduce confidence if not similar
                //    };
                //})
                //        .OrderByDescending(x => x.Score)
                //        .FirstOrDefault();

                if (matches != null && matches.Score >= threshold)
                {
                    matchedCells.Add(new
                    {
                        original = cellValue,
                        matches
                    });
                }
            }
            return Json(new { success = true, matchedCells });
        }

        [HttpPost]
        public IActionResult SaveMappingConfig([FromBody] ExcelMappingConfig config)
        {
            if (config == null)
                return BadRequest("Invalid configuration data.");

            //foreach (var sheetName in config.Mappings.Keys)
            //{
            //    var mapping = new ExcelSheetMapping
            //    {
            //        ConfigId = newConfig.Id,
            //        SheetName = sheetName,
            //        HeaderRowIndex = config.HeaderRows.ContainsKey(sheetName) ? config.HeaderRows[sheetName] : 0,
            //        FieldMappingsJson = JsonSerializer.Serialize(config.Mappings[sheetName]),
            //        UnmappedFieldsJson = config.Unmapped.ContainsKey(sheetName)
            //            ? JsonSerializer.Serialize(config.Unmapped[sheetName])
            //            : "{}"
            //    };
            //    _context.ExcelSheetMappings.Add(mapping);
            //}

            //_context.SaveChanges();

            return Json(new { success = true, message = "Mapping configuration saved successfully!" });
        }
    }

    public class FuzzyRequest
    {        public List<string> Headers { get; set; }
    }

    public class WorksheetPreview
    {
        public string WorksheetName { get; set; } = string.Empty;
        public List<List<string>> Rows { get; set; } = new();
    }

    public class ExcelPreviewModel
    {
        public List<string> WorksheetNames { get; set; } = new();
        public List<WorksheetPreview> Sheets { get; set; } = new();
    }

    // DTO for binding
    public class ExcelMappingConfig
    {
        public Dictionary<string, int> HeaderRows { get; set; } = new();
        public Dictionary<string, Dictionary<string, string>> Mappings { get; set; } = new();
        public Dictionary<string, Dictionary<string, UnmappedField>> Unmapped { get; set; } = new();
    }

    public class UnmappedField
    {
        public string Action { get; set; } = "ignore"; // "ignore" or "default"
        public string Value { get; set; } = "";
    }
}
