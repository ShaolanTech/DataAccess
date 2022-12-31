using ShaolanTech;
using System.Data.Common;

namespace ShaolanTech.Data
{
    public static class DBHelper
    {
        private static InstantiableDBHelper innerDBHelper;
        private static string connectionString = "";
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }

        public static DbProviderFactory CurrentDbProviderFactory
        {
            get
            {
                return innerDBHelper.CurrentDbProviderFactory;
            }
            set
            {
                innerDBHelper.CurrentDbProviderFactory = value;
            }
        }

        public static void CreateInstance()
        {
            innerDBHelper = new InstantiableDBHelper(ConnectionString);
        }
        static DBHelper()
        {

            //innerDBHelper = new InstantiableDBHelper(ConnectionString);
        }


        /// <summary>
        /// 创建一个数据库连接
        /// </summary>
        public static DbConnection CreateConnection()
        {
            return innerDBHelper.CreateConnection();
        }
        public static DbConnection CreateConnection(string connectionString)
        {
            return innerDBHelper.CreateConnection(connectionString);
        }
        public static DbParameter CreateParameter(string name, object value)
        {
            return innerDBHelper.CreateParameter(name, value);
        }
        /// <summary>
        /// 执行只返回一个值的SQL语句
        /// </summary>
        public static ResultInfo<T> ExecuteScalar<T>(Query query)
        {
            return ExecuteScalar<T>(query.CommandText, query.Parameters);
        }
        /// <summary>
        /// 执行只返回一个值的SQL语句
        /// </summary>
        public static ResultInfo<T> ExecuteScalar<T>(string commandText, params DbParameter[] parameters)
        {

            return innerDBHelper.ExecuteScalar<T>(commandText, parameters);
        }
        /// <summary>
        /// 获取一个DataReader
        /// </summary>
        public static DataReaderWrapper ExecuteReader(Query query)
        {
            return ExecuteReader(query.CommandText, query.Parameters);
        }
        /// <summary>
        /// 获取一个DataReader
        /// </summary>
        public static DataReaderWrapper ExecuteReader(string commandText, params DbParameter[] parameters)
        {
            return innerDBHelper.ExecuteReader(commandText, parameters);
        }
        /// <summary>
        /// 执行非选择性操作
        /// </summary>
        public static ResultInfo<int> ExecuteNonQuery(Query query)
        {

            return ExecuteNonQuery(query.CommandText, query.Parameters);
        }
        /// <summary>
        /// 执行非选择性操作
        /// </summary>
        public static ResultInfo<int> ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            return innerDBHelper.ExecuteNonQuery(commandText, parameters);
        }
#if NET461
        public static ResultInfo<DataSet> CreateDataSetSchemaBySQL(string sql, params DbParameter[] parameters)
        {
            return innerDBHelper.CreateDataSetSchemaBySQL(sql, parameters);
        }
        /// <summary>
        /// 返回一个数据集
        /// </summary>
        public static ResultInfo<DataSet> CreateDataSet(Query query)
        {
            return CreateDataSet(query.CommandText, query.Parameters);
        }
        /// <summary>
        /// 返回一个数据集
        /// </summary>
        public static ResultInfo<DataSet> CreateDataSet(string commandText, params DbParameter[] parameters)
        {
            return innerDBHelper.CreateDataSet(commandText, parameters);
        }
        /// <summary>
        /// 返回一个具备数据表结构的空数据集
        /// </summary>
        public static ResultInfo<DataSet> CreateDataSetSchema(string tableName)
        {
            return innerDBHelper.CreateDataSetSchema(tableName);
        }
        /// <summary>
        /// 更新数据集，包括添加、修改、删除
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="tableName">对应的表名</param>
        /// <returns></returns>
        public static ResultInfo UpdateDataSet(DataSet ds, string tableName)
        {
            return innerDBHelper.UpdateDataSet(ds, tableName);
        }
#endif
        /// <summary>
        /// 检测表是否存在
        /// </summary>
        public static bool CheckTableExist(string tableName)
        {
            return innerDBHelper.CheckTableExist(tableName);
        }
    
    }
}
