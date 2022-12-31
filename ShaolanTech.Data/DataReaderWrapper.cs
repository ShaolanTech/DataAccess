using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaolanTech.Data
{
    /// <summary>
    /// DataReader和DataConnection的封装类
    /// </summary>
    public class DataReaderWrapper : IDisposable
    {
        public bool OperationDone { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// 进行回收时的回调函数
        /// </summary>
        public Action OnDisposeCallback { get; set; }
        /// <summary>
        /// 获取对应的DataReader
        /// </summary>
        public DbDataReader DataReader { get; set; }
        private DbConnection connection;
        public DbConnection Connection { get { return this.connection; } }

        public string SingleViewName { get; set; }
        public Func<long> SingleViewCount { get; set; }

        private bool inTransaction = false;
         

        public DataReaderWrapper(DbDataReader reader=null, DbConnection con = null,bool inTransaction=false)
        {
            this.inTransaction = inTransaction;
            this.DataReader = reader;
            
            this.connection = con;
        }

        public bool Read()
        {
            bool result = false;
            try
            {
                result = this.DataReader.Read();
            }
            catch (Exception ex)
            {

                
            }
           

            return result;
        }
        public T ReadObject<T>(int index)
        {
            return this.DataReader.GetFieldValue<T>(index);
        }
        public T ReadStringToObject<T>(int index)
        {
            var obj = this.DataReader[index];
            if (obj.Equals(DBNull.Value))
            {
                return obj.ToString().FromJsonString<T>();
            }
            else
            {
                return (T)obj;
            }
        }
        public T ReadStringToObject<T>(string column)
        {
            var obj = this.DataReader[column];
            if (obj.Equals(DBNull.Value))
            {
                return obj.ToString().FromJsonString<T>();
            }
            else
            {
                return (T)obj;
            }
        }
        public T Read<T>(int index)
        {
            var obj = this.DataReader[index];
            if (obj.Equals(DBNull.Value))
            {
                return default(T);
            }
            else
            {
                return (T)obj;
            }
        }
        public T ReadJson<T>(string column)
        {
            return this.DataReader.GetFieldValue<T>(this.DataReader.GetOrdinal(column));
        }
        public T ReadJson<T>(int column)
        {
            return this.DataReader.GetFieldValue<T>( column );
        }
        public string ReadStringDefaultEmpty(string column )
        {
            var r = this.Read<string>(column);
            if (r == null )
            {
                return "";
            }
            return r;
        }
        public string ReadString(string column,bool defaultEmptyString=false)
        {
            var r = this.Read<string>(column);
            if (r==null&&defaultEmptyString)
            {
                return "";
            }
            return r;
        }
        public int ReadInt(string column)
        {
            return Read<int>(column);
        }
        public bool ReadBoolean(string column)
        {
            return Read<bool>(column);
        }
        public T Read<T>(string column)
        {
            var obj = this.DataReader[column];
            if (obj.Equals(DBNull.Value))
            {
                return default(T);
            }
            else
            {
                return (T)obj;
            }
        }
        public void ReleaseReader()
        {
            if (this.DataReader!=null)
            {
                this.DataReader.Close();
            }
           
        }
        public void ReleaseConnection()
        {
            if (this.connection!=null)
            {
                this.connection.Close();
            }
            
        }
        /// <summary>
        /// 释放DataReader以及DataConnection所占用资源
        /// </summary>
        public void Dispose()
        {

            this.ReleaseReader();
            this.ReleaseConnection();
            if (this.OnDisposeCallback != null)
            {
                this.OnDisposeCallback();
            }
        }
    }
}
