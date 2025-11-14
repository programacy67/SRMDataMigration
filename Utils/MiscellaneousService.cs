using System.ComponentModel;
using System.Data;
using static SRMDataMigrationIgnite.Services.Repositories.DMExportViewEntitiesService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SRMDataMigrationIgnite.Utils
{
    public static class MiscellaneousService
    {
        public static DataTable ToDataTableFromList<T>(IEnumerable<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public static DataTable ToDataTableFromEnum<T>(IEnumerable<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var prop in props)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static List<object> GetColumnDefinitions(DataTable dt, List<string?> columnsToHide, List<ViewEntityCategoryData> columnsAndCategories)
        {
            var columns = new List<object>();
            bool isHidden = false;
            string categoryName = string.Empty;
            foreach (DataColumn col in dt.Columns)
            {
                string igType = col.DataType.Name switch
                {
                    "Int32" => "number",
                    "Int64" => "number",
                    "Decimal" => "number",
                    "Double" => "number",
                    "Boolean" => "bool",
                    "DateTime" => "date",
                    _ => "string"
                };

                isHidden = false;
                if (columnsToHide != null)
                    isHidden = (from ch in columnsToHide where ch.Equals(col.ColumnName) select ch).Any();

                categoryName = (from cc in columnsAndCategories where cc.Title.Equals(col.ColumnName) select cc.CategoryTitle).FirstOrDefault();

                columns.Add(new
                {
                    headerText = col.ColumnName,
                    key = col.ColumnName,
                    dataType = igType,
                    hidden = isHidden,
                    category = categoryName
                });
            }
            return columns;
        }
    }
}
