using Npgsql;
using NpgsqlTypes;
using ShaolanTech;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShaolanTech.Data.Postgresql
{
    public static class PostgresqlDataContextExtensions
    {
        /// <summary>
        /// 创建一个准备好的数据操作连接
        /// </summary>
        /// <param name="commandText">SQL语句 </param>
        /// <param name="parameters">参数列表，不带值 </param>
        /// <returns></returns>
        public static PostgresqlExecutePlan CreatePlan(this PostgresqlDataContext context, string commandText, params NpgsqlDbType[] types)
        {
            PostgresqlExecutePlan result = null;
            var connection = (NpgsqlConnection)context.DBHelper.CreateConnection();
            connection.Open();
            var cmd = connection.CreateCommand();

            cmd.CommandText = commandText;
            try
            {
                if (types != null && types.Length != 0)
                {
                    cmd.Parameters.AddRange(types.Select(t => new NpgsqlParameter() { NpgsqlDbType = t }).ToArray());
                }
                cmd.Prepare();
                result = new PostgresqlExecutePlan(connection, cmd);
            }
            catch (Exception ex)
            {


            }
            return result;
        }
        public static PostgresqlBinaryWriter CreateBinaryWriter(this PostgresqlDataContext context, string table, params string[] fields)
        {
            return new PostgresqlBinaryWriter((NpgsqlConnection)context.DBHelper.CreateConnection(), table, fields);
        }
    }
    public class PostgresqlDataContext : DataContextBase
    {

        static PostgresqlDBParameterTypeMappingProvider pgMappingProvider = new PostgresqlDBParameterTypeMappingProvider();
        public PostgresqlDataContext(string connectionString) : base(connectionString, Npgsql.NpgsqlFactory.Instance)
        {
            Npgsql.NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
            this.mappingProvider = pgMappingProvider;
        }
        public async Task<ResultInfo<int>> ExecuteNonQueryV2Async(string commandText, params object[] values)
        {
            var p = this.CreatePositionalParameterCollection();
            p.AddParameters(values);
            return await this.DBHelper.ExecuteNonQueryAsync(commandText, p.ToArray());
        }
        public ResultInfo<DataSet> CreateDataSetV2(string commandText, params object[] values)
        { 
            var p = this.CreatePositionalParameterCollection();
            p.AddParameters(values); 
            return this.DBHelper.CreateDataSet(commandText, p.ToArray());
        }
        public ResultInfo<T>ExecuteScalarV2<T>(string commandText, params object[] values)
        {
            var p = this.CreatePositionalParameterCollection();
            p.AddParameters(values);
            return this.ExecuteScalar<T>(commandText, p.ToArray());
        }

 
        public DataReaderWrapper ExecuteReaderV2(string commandText, params object[] values)
        {
            var p = this.CreatePositionalParameterCollection();
            p.AddParameters(values);
            return this.DBHelper.ExecuteReader(commandText, p.ToArray());
        }
        public T FindTableObjectV2<T>(string commandText, params object[] values) where T:TableObject
        {
           return FindTableObjectsV2<T>(commandText, values).FirstOrDefault();
        }
        public  IEnumerable<T> FindTableObjectsV2<T>(string commandText, params object[] values)
        {
            var sql = "";
            var type = typeof(T);

            if (commandText.IsNotNullOrEmpty())
            {
                sql = commandText;
            }
            else
            {
                var tableName = type.Name;
                var attr = type.GetCustomAttribute<TableMappingAttribute>();
                if (attr != null && attr.MappingTableName.IsNotNullOrEmpty())
                {
                    tableName = attr.MappingTableName;
                }
                sql = $"select * from {tableName}";
            }
            var p = this.CreatePositionalParameterCollection();
            p.AddParameters(values);
            using (var reader = this.ExecuteReader(sql, p.ToArray()))
            {
                if (reader == null)
                {
                    yield break;
                }
                while (reader.Read())
                {
                    T item = default(T);
                    ExpandoObject row = new ExpandoObject();
                    for (int i = 0; i < reader.DataReader.FieldCount; i++)
                    {
                        var name = reader.DataReader.GetName(i);
                        var ft = reader.DataReader.GetDataTypeName(i).ToLower();

                        if (ft.Contains("json"))
                        {
                            try
                            {
row.SetPropery(name, (object)reader.DataReader[i].ToString().FromJsonString());
                            }
                            catch (Exception ex)
                            {

                                 
                            }
                            
                        }
                        else
                        {
                            var value = reader.DataReader[name];
                            if (ft.Contains("array"))
                            {
                                row.SetPropery(name, (Array)value);
                            }
                            else
                            {
                                if (ft.Contains("tsvector"))
                                {
                                    row.SetPropery(name, "");
                                }
                                else
                                {
                                    row.SetPropery(name, value);
                                }
                            }
                        }
                    }
                    try
                    {
var json = row.ToJsonString();
                    item = json.FromJsonString<T>();
                    if (item is TableObject)
                    {
                        ((TableObject)(object)item).Context = this;
                    }
                    }
                    catch (Exception ex)
                    {

                         
                    }
                    
                    yield return item;
                }
            }
        }

        public override bool ColumnExist(string tableName, string columnName)
        {
            return this.Exists($"select * from information_schema.columns where table_name = '{tableName.ToLower()}' and column_name='{columnName.ToLower().Remove("\"")}'");

        }

        public override async Task<ResultInfo> CreateMaterializedView(string viewName, string querySql, params DbParameter[] parameters)
        {
            ResultInfo result = new ResultInfo();
            result = await this.ExecuteNonQueryAsync($"create MATERIALIZED view {viewName} as   {querySql}", parameters);
            return result;
        }

        public override async Task<ResultInfo> CreateView(string viewName, string sql)
        {
            return await this.ExecuteNonQueryAsync($"create or replace view {viewName} as {sql}");
        }

        public async override Task<ResultInfo> EnableIndex(string tableName, bool enabled)
        {
            ResultInfo result = new ResultInfo();
            result = await this.ExecuteNonQueryAsync(@"
UPDATE pg_index
SET indisready=:p0
WHERE indrelid = (
    SELECT oid
    FROM pg_class
    WHERE relname=:p1
);
", this.NewParams(tableName, enabled));
            return result;
        }

        public override async Task<ResultInfo> EnableTableIndexAsync(string tableName, bool enableIndex)
        {
            var sql = "UPDATE pg_index "
            .Append(" SET indisready = $1 ")
            .Append(" WHERE indrelid = ( ")
            .Append("     SELECT oid ")
            .Append("     FROM pg_class ")
            .Append("     WHERE relname = $2 ")
            .Append(" ); ");
            return await this.ExecuteNonQueryV2Async(sql.ToString(), enableIndex, tableName.ToLower());
        }

        public override void EnsureColumn(string tableName, string columnName, string type, string comment = "")
        {
            this.ExecuteNonQuery($"alter table {tableName} add column IF NOT EXISTS {columnName} {type};comment on column {tableName}.{columnName} is '{comment.Replace("'", "''")}'");
        }

        public override async Task<ResultInfo> EnsureSchema(string name)
        {
            return await this.ExecuteNonQueryV2Async($"create schema if not exists {name.ToLower()}");
        }

        public override void EnsureTableObject(Type type1, bool isParitionTable = false, string paritionKey = "")
        {
            throw new NotImplementedException();
        }

        public override bool Exists(string sql, params DbParameter[] parameters)
        {
            sql = $"select exists({sql})";
            var r = this.ExecuteScalar<bool>(sql, parameters);
            return r.AdditionalData;
        }

        public override IEnumerable<T> FindTableObjects<T>(string commandText, DbParameter[] parameters)
        {
            var sql = "";
            var type = typeof(T);

            if (commandText.IsNotNullOrEmpty())
            {
                sql = commandText;
            }
            else
            {
                var tableName = type.Name;
                var attr = type.GetCustomAttribute<TableMappingAttribute>();
                if (attr != null && attr.MappingTableName.IsNotNullOrEmpty())
                {
                    tableName = attr.MappingTableName;
                }
                sql = $"select * from {tableName}";
            }
            using (var reader = this.ExecuteReader(sql, parameters))
            {
                if (reader == null)
                {
                    Console.WriteLine("reader null");
                }
                while (reader.Read())
                {
                    T item = default(T);
                    ExpandoObject row = new ExpandoObject();
                    for (int i = 0; i < reader.DataReader.FieldCount; i++)
                    {
                        var name = reader.DataReader.GetName(i);
                        var ft = reader.DataReader.GetDataTypeName(i).ToLower();

                        if (ft.Contains("json"))
                        {
                            row.SetPropery(name, (object)reader.DataReader[i].ToString().FromJsonString());
                            //row.SetPropery(name,reader.DataReader.GetFieldValue<ExpandoObject>(i));

                        }
                        else
                        {
                            var value = reader.DataReader[name];
                            if (ft.Contains("array"))
                            {
                                row.SetPropery(name, (Array)value);
                            }
                            else
                            {
                                if (ft.Contains("tsvector"))
                                {
                                    row.SetPropery(name, "");
                                }
                                else
                                {
                                    row.SetPropery(name, value);
                                }
                            }
                        }
                    }
                    var json = row.ToJsonString();
                    item = json.FromJsonString<T>();
                    if (item is TableObject)
                    {
                        ((TableObject)(object)item).Context = this;
                    }
                    yield return item;
                }
            }
        }
        

        public override IEnumerable<T> FindTableObjectsByID<T>(List<string> ids, params string[] fields)
        {
            var type = typeof(T);
            var sql = "";
            var attr = type.GetCustomAttribute<TableMappingAttribute>();
            if (attr == null)
            {
                throw new Exception("表需要通过TableMappingAttribute进行修饰");
            }
            var tableName = type.Name;
            if (attr.MappingTableName.IsNotNullOrEmpty())
            {
                tableName = attr.MappingTableName;
            }
            StringBuilder sb = new StringBuilder();
            if (fields != null && fields.Length != 0)
            {
                sb.Append($"select {string.Join(",", fields)} from {attr.Schema}.{tableName} where _id in(select unnest(:p0::text[]))");
            }
            else
            {
                sb.Append($"select * from {attr.Schema}.{tableName} where  _id in(select unnest(:p0::text[]))");
            }

            return this.FindTableObjects<T>(sb.ToString(), this.NewParams(ids));
        }

        public override List<string> GetAllIndex(string tableName)
        {
            var sql = $"select indexname from pg_indexes where  tablename='{tableName.ToLower()}'";
            var ds = this.CreateDataSet(sql);
            return ds.AdditionalData.Tables[0].Rows.Cast<DataRow>().Select(r => r[0].ToString()).ToList();
        }

        public override List<string> GetTableNames(string prefix, string schema = "public")
        {
            var tables = this.CreateDataSet("select table_name from information_schema.tables where table_name~(:p0) and table_schema=:p1", this.NewParams(prefix, schema));
            List<string> result = new List<string>();
            foreach (var row in tables.ReadRow())
            {
                result.Add(row.Read<string>("table_name"));
            }
            return result;
        }


        public override bool IndexExist(string indexName)
        {
            return this.Exists($"select * from pg_indexes where   indexname='{indexName.ToLower()}'");
        }

        public override async Task<ResultInfo> InsertTableObjectsAsync(Type type, params TableObject[] objects)
        {
            return await this.UpsertTableObjectsAsync(type, objects.ToList(), null, UpsertConflictMode.DoNothing, useUpsert: false);
        }

        public override bool MaterializedViewExist(string viewName)
        {
            var sql = $"SELECT * FROM pg_catalog.pg_class c JOIN pg_namespace n ON n.oid = c.relnamespace WHERE c.relkind = 'm' and c.relname='{viewName.ToLower()}'";
            return this.Exists(sql);
        }

        public override async Task<ResultInfo> RefreshMaterializedView(string viewName)
        {
            ResultInfo result = new ResultInfo();
            if (this.MaterializedViewExist(viewName))
            {
                result = await this.ExecuteNonQueryAsync($"refresh MATERIALIZED view  {viewName} ");
            }
            return result;
        }

        public override bool TableExist(string tableName, string schema = "public")
        {
            var sql = $"select count(*) from information_schema.tables where table_name='{tableName.Trim().ToLower()}' and table_schema='{schema.Trim().ToLower()}'";
            var r = this.DBHelper.ExecuteScalar<long>(sql);
            return r.AdditionalData != 0;
        }

        public override bool TableIsInUsing(string tableName)
        {
            var r = this.ExecuteScalar<long>("select count(*) from pg_stat_activity where query~*(:p0) and state='active'", this.NewParams(tableName));
            return r.AdditionalData == 1;
        }

        public override bool TableIsNotInUsing(string tableName)
        {
            var r = this.ExecuteScalar<long>("select count(*) from pg_stat_activity where query~*(:p0) and state='active'", this.NewParams(tableName));
            return r.AdditionalData == 0;
        }

        public override async Task<ResultInfo> UpdateTableObjectFieldsAsync(Type type, List<TableObject> items, params string[] fields)
        {
            ResultInfo result = new ResultInfo();
            if (items.Count == 0)
            {
                return result;
            }
            try
            {
                type = items[0].GetType();
                var attr = type.GetCustomAttribute<TableMappingAttribute>();
                if (attr == null)
                {
                    throw new Exception("表需要通过TableMappingAttribute进行修饰");
                }

                var tableName = type.Name;
                if (attr.MappingTableName.IsNotNullOrEmpty())
                {
                    tableName = attr.MappingTableName;
                }
                StringBuilder sb = new StringBuilder();
                List<DbParameter> parameters = new List<DbParameter>();
                int index = 0;
                var runtimeProperties = DBTypeMappings.GetProperties(type);
                foreach (var item in items)
                {
                    sb.Append($"update {attr.Schema}.{tableName} set ");
                    foreach (var field in fields)
                    {


                        var property = runtimeProperties.FirstOrDefault(p => p.Name.ToLower() == field.ToLower());
                        if (property != null)
                        {
                            if (property.GetCustomAttribute<ColumnMappingAttribute>() != null && property.GetCustomAttribute<ColumnMappingAttribute>().Quotes)
                            {
                                sb.Append($"\"{field.ToLower()}\"=:p{index},");
                            }
                            else
                            {
                                sb.Append($" {field}=:p{index},");
                            }
                            var value = item.GetProperty(field);
                            var p = DBTypeMappings.CreateParameter(type, field);
                            if (value == null)
                            {
                                value = DBNull.Value;
                            }

                            parameters.Add(p);
                            p.ParameterName = $":p{index}";
                            p.NpgsqlValue = value;
                        }
                        index++;
                    }
                    sb.RemoveLast().Append($" where _id='{item._id}';");
                }
                result = await this.ExecuteNonQueryAsync(sb.ToString(), parameters.ToArray());
            }
            catch (Exception ex)
            {

                result.OperationDone = false;
                result.Message = ex.Message + "--" + ex.StackTrace;
            }
            return result;
        }

        public override async Task<ResultInfo> UpsertTableObjectsAsync(Type type, List<TableObject> objects, List<string> conflictFields, UpsertConflictMode mode = UpsertConflictMode.DoNothing, List<string> updateFieldNames = null, string tableName = null, bool useUpsert = true, bool forceRetry = false)
        {
            if (objects.Count == 0)
            {
                return new ResultInfo();
            }
            var objectName = type.Name;
            StringBuilder sb = new StringBuilder();
            ResultInfo result = new ResultInfo();
            var p = this.CreateParameterCollection();
            //此处已经考虑到了使用JsonIgnoreAttribute的情况，runtimeProperties中不包括已经被JsonIgnoreAttribute修饰过的属性
            var runtimeProperties = DBTypeMappings.GetProperties(type);
            var tableAttr = type.GetCustomAttribute<TableMappingAttribute>();
            string schema = "";
            if (tableName.IsNullOrEmpty())
            {

                if (tableAttr.MappingTableName.IsNotNullOrEmpty())
                {
                    tableName = tableAttr.MappingTableName;
                }
                else
                {
                    tableName = objectName;
                }
                if (tableAttr.Schema.IsNotNullOrEmpty())
                {
                    schema = $"{tableAttr.Schema}.";
                }
            }

            sb.Append($"insert into {schema}{tableName} (");

            foreach (var prop in runtimeProperties)
            {
                var name = prop.Name;

                var attr = prop.GetCustomAttribute<PostgresqlColumnMappingAttribute>();
                if (attr != null && attr.Quotes)
                {
                    name = $"\"{name}\"";
                }
                sb.Append($"{prop.Name},");

            }
            sb.RemoveLast().Append(")values");


            foreach (var obj in objects)
            {
                if (obj._id.IsNullOrEmpty())
                {
                    obj._id = Guid.NewGuid().ToString();
                }
                obj.UpdateTime = DateTime.Now;
                sb.Append("(");
                foreach (var prop in runtimeProperties)
                {

                    var value = obj.GetProperty(prop.Name);
                    var attr = prop.GetCustomAttribute<PostgresqlColumnMappingAttribute>();
                    if (attr != null && attr.ColumnType != NpgsqlDbType.Unknown)
                    {
                        if (attr.ColumnType == NpgsqlDbType.TsVector)
                        {
                            if (value == null)
                            {
                                sb.Append($"{p.AddParameter(value)},");
                            }
                            else
                            {
                                sb.Append($"to_tsvector({p.AddParameter(value.ToString().GetJiebaToken())}),");
                            }

                        }
                        else
                        {
                            sb.Append($"{p.AddParameter(value, attr.ColumnType)},");
                        }

                    }
                    else
                    {
                        sb.Append($"{p.AddParameter(value)},");
                    }

                }
                sb.RemoveLast().Append("),");

            }
            sb.RemoveLast();
            if (useUpsert)
            {
                sb.Append($" on conflict ({string.Join(",", conflictFields)}) ");
                if (mode == UpsertConflictMode.DoNothing)
                {
                    sb.Append("do nothing");
                }
                else
                {
                    sb.Append($"do update set {string.Join(",", updateFieldNames.Select(f => $"{f}=excluded.{f}"))}");
                }
            }

            result = await this.ExecuteNonQueryAsync(sb.ToString(), p.Clone());
            if (forceRetry)
            {
                int times = 0;
                while (result.OperationDone == false)
                {
                    await Task.Delay(5000);
                    result = await this.ExecuteNonQueryAsync(sb.ToString(), p.Clone());
                    times++;
                    if (times == 10)
                    {
                        break;
                    }
                }
            }

            sb.Clear();
            p.Dispose();
            return result;
        }

        public override bool ViewExist(Type type)
        {
            var name = type.Name;
            var sql = $"select (1) from information_schema.views where table_name='{name.ToLower()}'";
            return this.Exists(sql);
        }
    }
}
