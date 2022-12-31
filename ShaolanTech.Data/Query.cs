using System.Data.Common;
using System.Text;


namespace ShaolanTech.Data
{
    /// <summary>
    /// 查询参数封装类
    /// </summary>
    public class Query
    {
        /// <summary>
        /// SQL语句
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// 参数列表
        /// </summary>
        public DbParameter[] Parameters { get; set; }

        /// <summary>
        /// 查询类型，默认DataSet
        /// </summary>
        public QueryType QueryType { get; set; }

        public Query()
        {
            this.QueryType =  QueryType.DataSet;
        }
        public Query (string commandText,DbParameter[]parameters=null)
        {
            this.CommandText = commandText;
            this.Parameters = parameters;
            this.QueryType = QueryType.DataSet;
        }
        public Query SetCommandText(string commandText)
        {
            this.CommandText = commandText;
            return this;
        }
        public Query SetParameters(params DbParameter[] parameters)
        {
            this.Parameters = parameters;
            return this;

        }
       
        public Query SetQueryType(QueryType type)
        {
            this.QueryType = type;
            return this;
        }

        /// <summary>
        /// 获取当前查询的唯一表达值
        /// </summary>
        /// <returns></returns>
        public string GetUniqueValue()
        {
            StringBuilder sb = new StringBuilder(this.CommandText);
            if (this.Parameters!=null)
            {
                foreach (var p in this.Parameters)
                {
                    sb.Append(p.ParameterName).Append((int)p.DbType).Append(p.Value.ToJsonString());

                }
            }
            return SecurityUtil.GetMD5String(sb.ToString ());
        }
    }
}
