using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace ShaolanTech.Data.ManticoreSearch
{
    public class ManticoreDataContext : DataContextBase
    {
        static ManticoreDBParameterTypeMappingProvider provider = new ManticoreDBParameterTypeMappingProvider();
        public ManticoreDataContext(string connectionString) : base(connectionString, MySqlConnector.MySqlConnectorFactory.Instance)
        {
            this.mappingProvider = provider;
        }
        public ManticoreDataContext(string ip, int port) : base(new MySqlConnectionStringBuilder
        {
            Server=ip,Port=(uint)port,UserID="who",Password="pwd",Database="test",IgnoreCommandTransaction=true,Pooling=false,DefaultCommandTimeout=3000
        }.ConnectionString, MySqlConnector.MySqlConnectorFactory.Instance)
        {
             
        }
        public override bool ColumnExist(string tableName, string columnName)
        {
            throw new NotImplementedException();
        }

        public override Task<ResultInfo> CreateMaterializedView(string viewName, string querySql, params DbParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public override Task<ResultInfo> CreateView(string viewName, string sql)
        {
            throw new NotImplementedException();
        }

        public override Task<ResultInfo> EnableIndex(string tableName, bool enabled)
        {
            throw new NotImplementedException();
        }

        public override Task<ResultInfo> EnableTableIndexAsync(string tableName, bool enableIndex)
        {
            throw new NotImplementedException();
        }

        public override void EnsureColumn(string tableName, string columnName, string type, string comment = "")
        {
            throw new NotImplementedException();
        }

        public override Task<ResultInfo> EnsureSchema(string name)
        {
            throw new NotImplementedException();
        }

        public override void EnsureTableObject(Type type1, bool isParitionTable = false, string paritionKey = "")
        {
            throw new NotImplementedException();
        }

        public override bool Exists(string sql, params DbParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<T> FindTableObjects<T>(string commandText, DbParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<T> FindTableObjectsByID<T>(List<string> ids, params string[] fields)
        {
            throw new NotImplementedException();
        }

        public override List<string> GetAllIndex(string tableName)
        {
            throw new NotImplementedException();
        }

        public override List<string> GetTableNames(string prefix, string schema = "public")
        {
            List<string> result = new List<string>();

            var dsTables = this.DBHelper.CreateDataSet("show tables");
            foreach (var row in dsTables.ReadRow())
            {
                var name = row.ReadString("index");
                if (name.IsMatch(prefix))
                {
                    result.Add(name);
                }
            }
            return result;
        }

        public override bool IndexExist(string indexName)
        {
            throw new NotImplementedException();
        }

        public override Task<ResultInfo> InsertTableObjectsAsync(Type type, params TableObject[] objects)
        {
            throw new NotImplementedException();
        }

        public override bool MaterializedViewExist(string viewName)
        {
            throw new NotImplementedException();
        }

        public override Task<ResultInfo> RefreshMaterializedView(string viewName)
        {
            throw new NotImplementedException();
        }

        public override bool TableExist(string tableName, string schema = "public")
        {
            bool result = false;
            var dsTables = this.DBHelper.CreateDataSet("show tables");
            foreach (var row in dsTables.ReadRow())
            {
                var name = row.ReadString("index");
                if (name.ToLower() == tableName.ToLower())
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public override bool TableIsInUsing(string tableName)
        {
            throw new NotImplementedException();
        }

        public override bool TableIsNotInUsing(string tableName)
        {
            throw new NotImplementedException();
        }

        public override Task<ResultInfo> UpdateTableObjectFieldsAsync(Type type, List<TableObject> items, params string[] fields)
        {
            throw new NotImplementedException();
        }

        public override Task<ResultInfo> UpsertTableObjectsAsync(Type type, List<TableObject> objects, List<string> conflictFields, UpsertConflictMode mode = UpsertConflictMode.DoNothing, List<string> updateFieldNames = null, string tableName = null, bool useUpsert = true, bool forceRetry = false)
        {
            throw new NotImplementedException();
        }

        public override bool ViewExist(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
