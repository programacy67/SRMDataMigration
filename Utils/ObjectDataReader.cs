using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection;

namespace SRMDataMigrationIgnite.Utils
{
    public class ObjectDataReader<T> : IDataReader
    {
        private readonly IEnumerator<T> _dataEnumerator;
        private readonly List<PropertyInfo> _properties;

        public ObjectDataReader(IEnumerable<T> data)
        {
            _dataEnumerator = data.GetEnumerator();
            _properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead).ToList();
        }

        public int FieldCount => _properties.Count;

        public object GetValue(int i)
        {
            return _properties[i].GetValue(_dataEnumerator.Current) ?? DBNull.Value;
        }

        public string GetName(int i) => _properties[i].Name;

        public Type GetFieldType(int i) => _properties[i].PropertyType;

        public bool Read() => _dataEnumerator.MoveNext();

        public int GetOrdinal(string name)
        {
            for (int i = 0; i < _properties.Count; i++)
            {
                if (_properties[i].Name == name)
                    return i;
            }
            return -1;
        }

        public int GetValues(object[] values)
        {
            int count = Math.Min(values.Length, _properties.Count);
            for (int i = 0; i < count; i++)
            {
                values[i] = GetValue(i);
            }
            return count;
        }

        public void Close()
        {
            // No unmanaged resources, just dispose the enumerator
            _dataEnumerator.Dispose();
        }

        public void Dispose()
        {
            Close();
        }

        public object this[int i] => GetValue(i);
        public object this[string name] => GetValue(GetOrdinal(name));

        #region Not Implemented (can be extended as needed)
        public bool NextResult() => false;
        public bool IsDBNull(int i) => GetValue(i) == DBNull.Value;
        public int Depth => 1;
        public bool IsClosed => false;
        public int RecordsAffected => -1;
        public DataTable GetSchemaTable() => throw new NotSupportedException();

        public bool GetBoolean(int i) => (bool)GetValue(i);
        public byte GetByte(int i) => (byte)GetValue(i);
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => throw new NotSupportedException();
        public char GetChar(int i) => (char)GetValue(i);
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) => throw new NotSupportedException();
        public IDataReader GetData(int i) => throw new NotSupportedException();
        public string GetDataTypeName(int i) => GetFieldType(i).Name;
        public DateTime GetDateTime(int i) => (DateTime)GetValue(i);
        public decimal GetDecimal(int i) => (decimal)GetValue(i);
        public double GetDouble(int i) => (double)GetValue(i);
        public float GetFloat(int i) => (float)GetValue(i);
        public Guid GetGuid(int i) => (Guid)GetValue(i);
        public short GetInt16(int i) => (short)GetValue(i);
        public int GetInt32(int i) => (int)GetValue(i);
        public long GetInt64(int i) => (long)GetValue(i);
        public string GetString(int i) => (string)GetValue(i);
        #endregion
    }
}
