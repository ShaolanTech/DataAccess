using ShaolanTech;
using ShaolanTech.Data.DBParameters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShaolanTech.Data
{
    public abstract class DataContextBase : IDisposable
    {
        /// <summary>
        /// 内置DBHelper
        /// </summary>
        public InstantiableDBHelper DBHelper { get; set; }

        
        public string ConnectionString
        {
            get
            {
                return this.DBHelper.ConnectionString;
            }
        }
        
        public static Dictionary<string, Type> TableObjects { get => tableObjects; }

        static Dictionary<string, Type> tableObjects = new Dictionary<string, Type>();
       

        public DataContextBase(string connectionString,DbProviderFactory factory)
        {
            this.DBHelper = new InstantiableDBHelper(connectionString);
            this.DBHelper.CurrentDbProviderFactory = factory;
            System.Text.EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
            UTF8Encoding.RegisterProvider(provider);
            Encoding.RegisterProvider(provider);
           
        }
        #region 数据库操作辅助函数

        /// <summary>
        /// 创建参数集合
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public ParameterCollection CreateParameterCollection(string prefix=":")
        {
            return new ParameterCollection(prefix) { provider = this.mappingProvider  };
        }

        /// <summary>
        /// 创建匿名参数集合
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public ObjectParameterCollection CreatePositionalParameterCollection(string prefix = "$")
        {
            return new ObjectParameterCollection(prefix) { provider = this.mappingProvider };
        }
        /// <summary>
        /// 创建参数集合:p0,:p1.....
        /// </summary>
        /// <param name="parameters">参数值列表</param>
        /// <returns></returns>
        public DbParameter[] NewParams(params object[] parameters)
        {
            var p = new ParameterCollection() { provider = this.mappingProvider };
            if (parameters.IsNotNullOrEmpty())
            {
                foreach (var item in parameters)
                {
                    p.AddParameter(item);
                }
            }
            return p.ToArray();
        }


        public abstract bool Exists(string sql, params DbParameter[] parameters);
        
        /// <summary>
        /// 检测Schema是否存在
        /// </summary>
        /// <param name="name">Schema名称</param>
        /// <returns></returns>
        public abstract Task<ResultInfo> EnsureSchema(string name);
        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="viewName">视图名</param>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public abstract Task<ResultInfo> CreateView(string viewName, string sql);


        
        /// <summary>
        /// 检测索引是否存在
        /// </summary> 
        /// <param name="indexName">索引名</param>
        /// <returns></returns>
        public abstract bool IndexExist( string indexName);
        /// <summary>
        /// 获取全部索引名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract List<string> GetAllIndex(string tableName);

        /// <summary>
        /// 获取全部表名
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public abstract List<string> GetTableNames(string prefix, string schema = "public");
        /// <summary>
        /// 检测表是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TableExist(Type type)
        {
            var attr = type.GetCustomAttribute<TableMappingAttribute>();
            string tableName = type.Name;
            if (attr != null && attr.MappingTableName.IsNotNullOrEmpty())
            {
                tableName = attr.MappingTableName;
            }

            return this.TableExist(tableName);
        } 
        /// <summary>
        /// 检测表是否存在
        /// </summary>
        /// <returns></returns>
        public abstract bool TableExist(string tableName, string schema = "public");


        /// <summary>
        /// 检测视图是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract bool ViewExist(Type type);

        /// <summary>
        /// 刷新物化视图
        /// </summary>
        /// <param name="viewName">视图名</param>
        /// <returns></returns>
        public abstract Task<ResultInfo> RefreshMaterializedView(string viewName);

        /// <summary>
        /// 刷新或创建物化视图
        /// </summary>
        /// <param name="viewName">视图名</param>
        /// <param name="querySql">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public async Task<ResultInfo> RefreshOrCreateMaterializedView(string viewName, string querySql, params DbParameter[] parameters)
        {
            if (this.MaterializedViewExist(viewName))
            {
                return await this.RefreshMaterializedView(viewName);
            }
            else
            {
                return await this.CreateMaterializedView(viewName, querySql, parameters);
            }
        }
        public abstract Task<ResultInfo> CreateMaterializedView(string viewName, string querySql, params DbParameter[] parameters);
        public abstract bool MaterializedViewExist(string viewName);
        /// <summary>
        /// 检测视图是否存在
        /// </summary>

        /// <returns></returns>


        public async Task EnsureColumnAsync(string tableName, string columnName, string type)
        {
            if (!this.ColumnExist(tableName, columnName))
            {
                await this.ExecuteNonQueryAsync($"alter table {tableName} add column if not exists {columnName} {type}");
            }
        }
        public abstract void EnsureColumn(string tableName, string columnName, string type, string comment = "");
        public abstract bool ColumnExist(string tableName, string columnName);
        public bool ColumnExist(Type type, string columnName)
        {
            return this.ColumnExist(type.Name.ToLower(), columnName.ToLower());
        }


        public abstract Task<ResultInfo> EnableIndex(string tableName, bool enabled);

        public async Task<ResultInfo> DropIndex(string indexName)
        {
            ResultInfo result = new ResultInfo();
            if (this.IndexExist(indexName))
            {
                result = await this.ExecuteNonQueryAsync($"drop index {indexName.ToLower()}");
            }
            return result;
        } 
        #endregion




        /// <summary>
        /// 开启或禁用表索引 
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="enableIndex">是否开启索引</param>
        /// <returns></returns>
        public abstract Task<ResultInfo> EnableTableIndexAsync(string tableName, bool enableIndex);

       
 
        
 

         
      
        


        static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ensuredIndexColumns = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();




        static ConcurrentDictionary<Type, PropertyInfo[]> typeCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        protected DBParameterTypeMappingProvider mappingProvider;

        public async Task<ResultInfo> InsertTableObjectsAsync<T>(List<T> items) where T : TableObject
        {
            return await this.InsertTableObjectsAsync<T>(items.ToArray());
        }
        /// <summary>
        /// 执行Upsert操作
        /// </summary> 
        /// <param name="obj">数据对象</param>
        /// <param name="conflictFields">主键列</param>
        /// <param name="mode">Upsert冲突模式</param>
        /// <param name="updateFieldNames">执行更新操作时，对应更新的列名</param>
        /// <returns></returns>
        public async Task<ResultInfo> UpsertTableObjectsAsync<T>(T obj, List<string> conflictFields, UpsertConflictMode mode = UpsertConflictMode.DoNothing, List<string> updateFieldNames = null, string tableName = null, bool useUpsert = true, bool forceRetry = false) where T : TableObject
        {
            return await this.UpsertTableObjectsAsync(typeof(T), new List<TableObject> { obj }, conflictFields, mode, updateFieldNames, tableName, useUpsert, forceRetry);
        }
        /// <summary>
        /// 执行Upsert操作
        /// </summary> 
        /// <param name="objects">数据列表</param>
        /// <param name="conflictFields">主键列</param>
        /// <param name="mode">Upsert冲突模式</param>
        /// <param name="updateFieldNames">执行更新操作时，对应更新的列名</param>
        /// <returns></returns>
        public async Task<ResultInfo> UpsertTableObjectsAsync<T>(List<T> objects, List<string> conflictFields, UpsertConflictMode mode = UpsertConflictMode.DoNothing, List<string> updateFieldNames = null, string tableName = null, bool useUpsert = true, bool forceRetry = false) where T : TableObject
        {
            return await this.UpsertTableObjectsAsync(typeof(T), objects.Select(o => (TableObject)o).ToList(), conflictFields, mode, updateFieldNames, tableName, useUpsert, forceRetry);
        }
        /// <summary>
        /// 执行Upsert操作
        /// </summary>
        /// <param name="type">表对象类型</param>
        /// <param name="objects">数据列表</param>
        /// <param name="conflictFields">主键列</param>
        /// <param name="mode">Upsert冲突模式</param>
        /// <param name="updateFieldNames">执行更新操作时，对应更新的列名</param>
        /// <returns></returns>
        public abstract Task<ResultInfo> UpsertTableObjectsAsync(Type type, List<TableObject> objects, List<string> conflictFields, UpsertConflictMode mode = UpsertConflictMode.DoNothing, List<string> updateFieldNames = null, string tableName = null, bool useUpsert = true, bool forceRetry = false);
        public abstract Task<ResultInfo> InsertTableObjectsAsync(Type type, params TableObject[] objects);
        public async Task<ResultInfo> InsertTableObjectsAsync<T>(params T[] items) where T : TableObject
        {
            return await this.InsertTableObjectsAsync(typeof(T), items.Select(i => (TableObject)i).ToArray());
        }

       
        public async Task<ResultInfo> UpdateTableObjectFieldsAsync<T>(T item, params string[] fields) where T : TableObject, new()
        {
            if (fields == null)
            {
                var temp = new List<string>();
                foreach (var field in item.GetProperties())
                {
                    if (field.Key != "_id")
                    {
                        if (field.Value is string && field.Value == null)
                        {
                            item.SetProperty(field.Key, "");
                        }
                        temp.Add(field.Key);
                    }
                }
                fields = temp.ToArray();
            }
            return await this.UpdateTableObjectFieldsAsync(new List<T> { item }, fields);
        }
        public async Task<ResultInfo> UpdateTableObjectFieldsAsync<T>(List<T> items, params string[] fields) where T : TableObject, new()
        {
            return await this.UpdateTableObjectFieldsAsync(typeof(T), items.Select(t => (TableObject)t).ToList(), fields);
        }

        /// <summary>
        /// 批量更新对象表字段
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="items">对象列表</param>
        /// <param name="fields">要更新的字段列表</param>
        /// <returns></returns>
        public abstract Task<ResultInfo> UpdateTableObjectFieldsAsync(Type type, List<TableObject> items, params string[] fields);

        /// <summary>
        /// 通过对象ID查找对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public T FindTableObjectByID<T>(string id, params string[] fields) where T : TableObject, new()
        {
            var r = this.FindTableObjectsByID<T>(new List<string> { id }, fields).ToList().FirstOrDefault();
            if (r == null)
            {
                return default(T);
            }
            r._id = id;
            return r;
        }
        public abstract IEnumerable<T> FindTableObjectsByID<T>(List<string> ids, params string[] fields) where T : TableObject, new();


        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public abstract IEnumerable<T> FindTableObjects<T>(string commandText ,  DbParameter[] parameters) where T : new();
        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">参数值列表</param>
        /// <returns></returns>
        public virtual IEnumerable<T> FindTableObjects<T>(string commandText,params object[]parameters) where T : new()
        {
            return this.FindTableObjects<T>(commandText, this.NewParams(parameters));
        }
        


        /// <summary>
        /// 创建列表
        /// </summary>
        /// <typeparam name="T">Model的类型</typeparam>
        /// <returns></returns>
        public async Task EnsureTableObjectAsync<T>() where T : TableObject
        {
            await Task.Run(() =>
            {
                this.EnsureTableObject(typeof(T));
            });
        }
        /// <summary>
        /// 创建列表
        /// </summary>
        /// <param name="type">Model的类型</param>
        /// <returns></returns>
        public async Task EnsureTableObjectAsync(Type type)
        {
            await Task.Run(() =>
            {
                this.EnsureTableObject(type);
            });
        }



        /// <summary>
        /// 创建列表
        /// </summary>
        /// <param name="type">Model的类型</param>
        /// <returns></returns>
        public abstract void EnsureTableObject(Type type1, bool isParitionTable = false, string paritionKey = ""); 

        #region 关系SQL操作 
         
        /// <summary>
        /// 执行只返回一个值的SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        public virtual ResultInfo<T> ExecuteScalar<T>(Query query)
        {
            return this.DBHelper.ExecuteScalar<T>(query.CommandText, query.Parameters);
        }
        /// <summary>
        /// 执行只返回一个值的SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        public virtual ResultInfo<T> ExecuteScalar<T>(string commandText, params DbParameter[] parameters)
        {
            return this.DBHelper.ExecuteScalar<T>(commandText, parameters);
        }

        /// <summary>
        /// 获取一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        public virtual DataReaderWrapper ExecuteReader(string commandText, params DbParameter[] parameters)
        {
            return this.DBHelper.ExecuteReader(commandText, parameters);
        }
        
        /// <summary>
        /// 执行非选择性操作
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public ResultInfo<int> ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            return this.DBHelper.ExecuteNonQuery(commandText, parameters);
        }
        
        
         
        /// <summary>
        /// 执行非选择性操作
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public virtual async Task<ResultInfo<int>> ExecuteNonQueryAsync(string commandText, params DbParameter[] parameters)
        {
            return await this.DBHelper.ExecuteNonQueryAsync(commandText, parameters);
        }
        

        public async Task<ResultInfo<List<Dictionary<string, object>>>> ReadDicAsync(string commandText, params DbParameter[] parameters)
        {
            return await Task.Run<ResultInfo<List<Dictionary<string, object>>>>(() =>
            {
                return this.ReadDic(commandText, parameters);
            });
        }
        
        public virtual ResultInfo<DataSet> CreateDataSet(string commandText, params DbParameter[] parameters)
        {
            return this.DBHelper.CreateDataSet(commandText, parameters);
        }
         
        public async Task<ResultInfo<DataSet>> CreateDataSetAsync(string commandText, params DbParameter[] parameters)
        {
            return await Task.Run<ResultInfo<DataSet>>(() =>
            {
                return this.CreateDataSet(commandText, parameters);
            });

        }
        public ResultInfo<List<Dictionary<string, object>>> ReadDic(string commandText, params DbParameter[] parameters)
        {
            ResultInfo<List<Dictionary<string, object>>> result = new ResultInfo<List<Dictionary<string, object>>>();
            result.AdditionalData = new List<Dictionary<string, object>>();
            try
            {
                using (var reader = this.DBHelper.ExecuteReader(commandText, parameters))
                {
                    while (reader.DataReader.Read())
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.DataReader.FieldCount; i++)
                        {
                            var field = reader.DataReader.GetName(i);
                            if (row.ContainsKey(field) == false)
                            {
                                row.Add(field, reader.DataReader[i]);
                            }
                        }
                        result.AdditionalData.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {

                result.OperationDone = false;
                result.Message = ex.Message;
            }
            return result;
        }
        /// <summary>
        /// 检测指定数据表是否被占用
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public abstract bool TableIsInUsing(string tableName);
        /// <summary>
        /// 检测指定数据表是否没有被占用
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public abstract bool TableIsNotInUsing(string tableName);
        /// <summary>
        /// 检测表是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual bool TableExist<T>()
        {
            return this.TableExist(typeof(T));
        }

        /// <summary>
        /// 检测视图是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual bool ViewExist<T>()
        {
            return this.ViewExist(typeof(T));
        }
          



        public bool ColumnExist<T>(string columnName)
        {
            return this.ColumnExist(typeof(T), columnName);
        }
        public void Dispose()
        {

        }
        #endregion
    }
}
