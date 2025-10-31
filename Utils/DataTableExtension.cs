using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace SRMDataMigrationIgnite.Utils
{
    public static class DataTableExtension
    {
        public static void SetColumnsOrder(this DataTable table, params string[] columnNames)
        {
            int columnIndex = 0;
            foreach (string columnName in columnNames)
            {
                if (table.Columns.Contains(columnName))
                {
                    table.Columns[columnName].SetOrdinal(columnIndex);
                    columnIndex = columnIndex + 1;
                }
            }
        }
    }
}
