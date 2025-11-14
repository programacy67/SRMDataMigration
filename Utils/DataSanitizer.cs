using Microsoft.AspNetCore.Http.Json;
using System.Data;
using System.Text.Json;

namespace SRMDataMigrationIgnite.Utils
{
    public class DataSanitizer
    {
        private readonly string _nullReplacement;

        public DataSanitizer(IConfiguration config)
        {
            _nullReplacement = config["GridSettings:NullReplacementText"] ?? "";
        }

        public Dictionary<string, object?> CleanEntity(object entity)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var prop in entity.GetType().GetProperties())
            {
                var val = prop.GetValue(entity);
                if (val == null)
                    dict[prop.Name] = _nullReplacement;
                else if (val is not string && val.GetType().IsClass && !(val is ValueType))
                    dict[prop.Name] = val.ToString(); // flatten complex object
                else
                    dict[prop.Name] = val;
            }
            return dict;

            //Example of usage
            //var employees = await _context.Employees.ToListAsync();
            //var cleaned = employees.Select(e => _sanitizer.CleanEntity(e)).ToList();
        }

        public List<Dictionary<string, object?>> CleanDataTable(DataTable table, string? nullReplacement = null)
        {
            var sanitized = new List<Dictionary<string, object?>>();

            foreach (DataRow row in table.Rows)
            {
                var cleanRow = new Dictionary<string, object?>();

                foreach (DataColumn col in table.Columns)
                {
                    var value = row[col];

                    if (value == DBNull.Value || value == null)
                    {
                        cleanRow[col.ColumnName] = nullReplacement ?? null;
                    }
                    else if (value is string s)
                    {
                        cleanRow[col.ColumnName] = s.Trim();
                    }
                    else if (value.GetType().IsClass && !(value is ValueType))
                    {
                        // Handle objects (convert to string or JSON)
                        cleanRow[col.ColumnName] = value.ToString();
                    }
                    else
                    {
                        cleanRow[col.ColumnName] = value;
                    }
                }
                sanitized.Add(cleanRow);
            }
            return sanitized;
        }

        public JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
            };
        }
    }
}
