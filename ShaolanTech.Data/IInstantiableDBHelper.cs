using ShaolanTech;
using System;
using System.Data.Common;
namespace ShaolanTech.Data
{
    public interface IInstantiableDBHelper
    {
        bool CheckTableExist(string tableName,string dataBase="");
        string ConnectionString { get; set; }
        DbConnectionStringBuilder ConnectionStringBuilder { get; set; }
        System.Data.Common.DbConnection CreateConnection();
        System.Data.Common.DbConnection CreateConnection(string connectionString);
#if NET461
        Warensoft.EntLib.Common.ResultInfo UpdateDataSet(System.Data.DataSet ds, string tableName);
        Warensoft.EntLib.Common.ResultInfo<System.Data.DataSet> CreateDataSet(Query query);
        Warensoft.EntLib.Common.ResultInfo<System.Data.DataSet> CreateDataSet(string commandText, params System.Data.Common.DbParameter[] parameters);
        Warensoft.EntLib.Common.ResultInfo<System.Data.DataSet> CreateDataSetSchema(string tableName);
        Warensoft.EntLib.Common.ResultInfo<System.Data.DataSet> CreateDataSetSchemaBySQL(string sql, params System.Data.Common.DbParameter[] parameters);
#endif
        DbParameter CreateParameter(string name, object value);
        System.Data.Common.DbProviderFactory CurrentDbProviderFactory { get; set; }
        ResultInfo<int> ExecuteNonQuery(Query query);
        ResultInfo<int> ExecuteNonQuery(string commandText, params DbParameter[] parameters);
        DataReaderWrapper ExecuteReader(Query query);
        DataReaderWrapper ExecuteReader(string commandText, params DbParameter[] parameters);
        ResultInfo<T> ExecuteScalar<T>(Query query);
        ResultInfo<T> ExecuteScalar<T>(string commandText, params DbParameter[] parameters);
        
     
    }
}
