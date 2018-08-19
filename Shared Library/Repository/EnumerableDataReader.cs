using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;

namespace ZondervanLibrary.SharedLibrary.Repository
{
    public class EnumerableDataReader<TEntity> : IDataReader
    {
        private readonly IEnumerator<TEntity> _enumerator;
        private readonly Func<TEntity, object[]> _converter;
        private Boolean _isDisposed;
        private Boolean _moveNextResult;
        private object[] _dataRow;
        private Boolean _isFirst;

        public EnumerableDataReader(IEnumerable<TEntity> entities, Func<TEntity, object[]> converter)
        {
            Contract.Requires(entities != null);
            Contract.Requires(converter != null);

            _enumerator = entities.GetEnumerator();
            _converter = converter;

            // Get first result prematurely in order to determine field count.
            _moveNextResult = _enumerator.MoveNext();
            _isFirst = true;

            if (_moveNextResult)
            {
                _dataRow = _converter(_enumerator.Current);
                FieldCount = _dataRow.Length;
            }
            else
            {
                FieldCount = 0;
            }
        }

        public void Close()
        {
            Dispose();
        }

        public int Depth => throw new NotImplementedException();

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool IsClosed => _isDisposed;

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("EnumerableDataReader");

            if (!_isFirst)
            {
                _moveNextResult = _enumerator.MoveNext();

                if (_moveNextResult)
                {
                    _dataRow = _converter(_enumerator.Current);
                }
            }

            _isFirst = false;

            return _moveNextResult;
        }

        public int RecordsAffected => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int FieldCount { get; }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            if (FieldCount <= i)
                throw new IndexOutOfRangeException();
            else
                return (DateTime)_dataRow[i];
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("EnumerableDataReader");

            return _dataRow[i];
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return _dataRow[i] == null;
        }

        public object this[string name] => throw new NotImplementedException();

        public object this[int i] => throw new NotImplementedException();
    }

    
}
