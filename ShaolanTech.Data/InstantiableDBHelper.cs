using ShaolanTech;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace ShaolanTech.Data
{

    public class InstantiableDBHelper 
    {
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        DbProviderFactory currentFactory = null;

        public DbProviderFactory CurrentDbProviderFactory
        {
            get
            {

                return currentFactory;


            }
            set
            {
                this.currentFactory = value;
            }
        }

        public Func<DbDataAdapter> CreateDataAdapter { get; set; }
        public InstantiableDBHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }


        /// <summary>
        /// 创建一个数据库连接
        /// </summary>
        public DbConnection CreateConnection()
        {
            var con = CurrentDbProviderFactory.CreateConnection();
            con.ConnectionString = ConnectionString;

            return con;
        }
        public DbConnection CreateConnection(string connectionString)
        {
            var con = CurrentDbProviderFactory.CreateConnection();
            con.ConnectionString = connectionString;

            return con;
        }
        public DbParameter CreateParameter(string name, object value)
        {
            var p = this.CurrentDbProviderFactory.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            return p;
        }
        /// <summary>
        /// 执行只返回一个值的SQL语句
        /// </summary>
        public ResultInfo<T> ExecuteScalar<T>(Query query)
        {
            return ExecuteScalar<T>(query.CommandText, query.Parameters);
        }
        /// <summary>
        /// 执行只返回一个值的SQL语句
        /// </summary>
        public ResultInfo<T> ExecuteScalar<T>(string commandText, params DbParameter[] parameters)
        {
            ResultInfo<T> result = new ResultInfo<T>();
            using (var con = CreateConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = commandText;
                        if (parameters != null && parameters.Length != 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        con.Open();
                        cmd.CommandTimeout = short.MaxValue;
                        var obj= cmd.ExecuteScalar();
                        if (obj==DBNull.Value)
                        {
                            result.AdditionalData = default(T);
                        }
                        else
                        {
                            result.AdditionalData =  (T)obj;
                        }
                        
                        result.OperationDone = true;
                    }
                    catch (Exception ex)
                    {
                        result.OperationDone = false;
                        result.Message = ex.Message;
                         
                        GC.Collect();
                    }
                   

                }
            }
            return result;
        }
        /// <summary>
        /// 获取一个DataReader
        /// </summary>
        public DataReaderWrapper ExecuteReader(Query query)
        {
            return ExecuteReader(query.CommandText, query.Parameters);
        }
        /// <summary>
        /// 获取一个DataReader
        /// </summary>
        public DataReaderWrapper ExecuteReader(string commandText, params DbParameter[] parameters)
        {
            try
            {
                var con = CreateConnection();
                var cmd = con.CreateCommand();
                cmd.CommandText = commandText;
                DataReaderWrapper wrapper = null;
                if (parameters != null && parameters.Length != 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                try
                {
                    con.Open();
                    cmd.CommandTimeout = short.MaxValue;
                    var reader = cmd.ExecuteReader();
                    if (reader==null)
                    {

                    }
                    wrapper = new DataReaderWrapper(reader, con);
                }
                catch (Exception ex)
                {
                    if (con.State== ConnectionState.Open)
                    {
                        con.Dispose();
                    }
                    cmd.Dispose();
                    GC.Collect();
                   // Console.WriteLine(con.ConnectionString);
                }

               
                return wrapper;
            }
            catch (Exception ex)
            {
           
                GC.Collect();

            }
            return null;
        }
        /// <summary>
        /// 执行非选择性操作
        /// </summary>
        public ResultInfo<int> ExecuteNonQuery(Query query)
        {
            return ExecuteNonQuery(query.CommandText, query.Parameters);
        }
        /// <summary>
        /// 执行非选择性操作
        /// </summary>

        public ResultInfo<int> ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            ResultInfo<int> result = new ResultInfo<int>();
            if (commandText.IsNullOrEmpty())
            {
                return result;
            }
            using (var con = CreateConnection())
            {

                var cmd = con.CreateCommand();
                cmd.CommandText = commandText;
                if (parameters != null && parameters.Length != 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                 
                try
                { 
                    con.Open();
                    cmd.CommandTimeout = short.MaxValue;
                    result.AdditionalData = cmd.ExecuteNonQuery();
                    result.OperationDone = true; 
                }
                
                catch (Exception ex)
                {

                    if (parameters!=null)
                    {

                        parameters = null;
                    }
                    cmd.Dispose();
                    result.OperationDone = false;
                    result.Message = ex.Message; 
                    GC.Collect();
                }

            }
            return result;
        }


        /// <summary>
        /// 执行非选择性操作
        /// </summary>

        public async Task<ResultInfo<int>> ExecuteNonQueryAsync(string commandText, params DbParameter[] parameters)
        {
            if (commandText.IsNullOrEmpty())
            {

            }
            return await Task.Run<ResultInfo<int>>(() =>
            {
                return this.ExecuteNonQuery(commandText, parameters);
            });
        }
        public TransactionWrapper ExecuteNonQueryWithTransaction(string commandText, params DbParameter[] parameters)
        {
            TransactionWrapper result = new TransactionWrapper();
            var con = CreateConnection();


            var cmd = con.CreateCommand();
            cmd.CommandText = commandText;
            if (parameters != null && parameters.Length != 0)
            {
                cmd.Parameters.AddRange(parameters);
            }
            con.Open();
            var trans = con.BeginTransaction();
            cmd.Transaction = trans;
            result.Connection = con;
            result.Transaction = trans;
            try
            {


                cmd.CommandTimeout = short.MaxValue;
                cmd.ExecuteNonQuery();
                result.OperationDone = true;

            }
            catch (Exception ex)
            {

                cmd.Dispose();
                result.OperationDone = false;
                result.Message = ex.Message;

            }


            return result;
        }

        public async Task<TransactionWrapper> ExecuteNonQueryWithTransactionAsync(string commandText, params DbParameter[] parameters)
        {
            return await Task.Run<TransactionWrapper>(() => 
            {
                return this.ExecuteNonQueryWithTransaction(commandText,parameters);
            });
        }

        /// <summary>
        /// 返回一个数据集
        /// </summary>
        public ResultInfo<DataSet> CreateDataSet(Query query)
        {
            return CreateDataSet(query.CommandText, query.Parameters);
        }
        /// <summary>
        /// 返回一个数据集
        /// </summary>
        public ResultInfo<DataSet> CreateDataSet(string commandText, params DbParameter[] parameters)
        {
            ResultInfo<DataSet> result = new ResultInfo<DataSet>();
            using (var con = CreateConnection())
            {
                var cmd = con.CreateCommand();
                try
                {
                    
                    cmd.CommandText = commandText;
                    if (parameters != null && parameters.Length != 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    DbDataAdapter adpt = null;// CurrentDbProviderFactory.CreateDataAdapter();
                    if (CreateDataAdapter!=null)
                    {
                        adpt = CreateDataAdapter();
                    }
                    else
                    {
                        adpt = CurrentDbProviderFactory.CreateDataAdapter();
                    }
                    adpt.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    adpt.Fill(ds);
                    result.OperationDone = true;
                    result.AdditionalData = ds;
                }
                catch (Exception ex)
               {
                    result.OperationDone = false;
                    cmd.Dispose();
                    result.Message = ex.Message; 
                    GC.Collect();
                }
            }
            return result;
        }
        /// <summary>
        /// 返回一个具备数据表结构的空数据集
        /// </summary>
        public ResultInfo<DataSet> CreateDataSetSchemaBySQL(string sql, params DbParameter[] parameters)
        {
            ResultInfo<DataSet> result = new ResultInfo<DataSet>();
            using (var con = CreateConnection())
            {

                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                if (parameters != null && parameters.Length != 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                var adpt = CurrentDbProviderFactory.CreateDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                try
                {
                    adpt.FillSchema(ds, SchemaType.Mapped);
                    result.AdditionalData = ds;
                    result.OperationDone = true;
                }
                catch (Exception ex)
                {
                    cmd.Dispose();
                    adpt.Dispose();
                    ds.Clear();
                    ds.Dispose();
                    result.OperationDone = false;
                    result.Message = ex.Message; 
                }
            }
            return result;
        }
        /// <summary>
        /// 返回一个具备数据表结构的空数据集
        /// </summary>
        public ResultInfo<DataSet> CreateDataSetSchema(string tableName)
        {
            ResultInfo<DataSet> result = new ResultInfo<DataSet>();
            using (var con = CreateConnection())
            {

                var cmd = con.CreateCommand();
                cmd.CommandText = "select * from " + tableName;
                var adpt = CurrentDbProviderFactory.CreateDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                try
                {
                    adpt.FillSchema(ds, SchemaType.Mapped);
                    result.AdditionalData = ds;
                    result.OperationDone = true;
                }
                catch (Exception ex)
                {
                    cmd.Dispose();
                    adpt.Dispose();
                    ds.Clear();
                    ds.Dispose();
                    result.OperationDone = false;
                    result.Message = ex.Message; 
                }
            }
            return result;
        }
        /// <summary>
        /// 更新数据集，包括添加、修改、删除
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="tableName">对应的表名</param>
        /// <returns></returns>
        public ResultInfo UpdateDataSet(DataSet ds, string tableName)
        {
            ResultInfo result = new ResultInfo();
            using (var con = this.CreateConnection())
            {
                try
                {
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "select * from " + tableName;
                    var adpt = this.CurrentDbProviderFactory.CreateDataAdapter();
                    adpt.SelectCommand = cmd;
                    var builder = this.CurrentDbProviderFactory.CreateCommandBuilder();
                    builder.DataAdapter = adpt;
                    builder.ConflictOption = ConflictOption.OverwriteChanges;
                    adpt.InsertCommand = builder.GetInsertCommand();
                    adpt.UpdateCommand = builder.GetUpdateCommand();
                    adpt.DeleteCommand = builder.GetDeleteCommand();
                    adpt.Update(ds);
                    result.OperationDone = true;
                }
                catch (Exception ex)
                {
                    result.OperationDone = false;
                    result.Message = ex.Message; 
                }
            }
            return result;
        }
 
        /// <summary>
        /// 检测表是否存在
        /// </summary>
        public bool CheckTableExist(string tableName, string dataBase = "")
        {
            string sql = "";
            switch (this.CurrentDbProviderFactory.ToString())
            {
                case "MySql.Data.MySqlClient.MySqlClientFactory":
                    sql = string.Format("select count(*) from information_schema.tables where  table_name='{0}' and Table_SCHEMA='{1}'", tableName, dataBase);
                    break;
                case "System.Data.SqlClient.SqlClientFactory":
                    sql = string.Format("SELECT count(*) FROM sys.Tables  where type='U' and name='{0}'", tableName);
                    break;
                case "Oracle.Data.OracleClient.OracleClientFactory":
                    sql = string.Format("select count(*) from tab where tname='{0}'", tableName.ToUpper());
                    break;
                default:
                    break;
            }

            var result = this.ExecuteScalar<object>(sql);
            if (result.OperationDone)
            {
                return double.Parse(result.AdditionalData.ToString()) == 0 ? false : true;
            }
            else
            {

            }
            return false;
        }

        public DbConnectionStringBuilder ConnectionStringBuilder
        {
            get
            {
                var cb = this.CurrentDbProviderFactory.CreateConnectionStringBuilder();
                cb.ConnectionString = this.ConnectionString;
                return cb;
            }
            set
            { }
        }
    }
}
