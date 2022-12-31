using Newtonsoft.Json;
using ShaolanTech;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShaolanTech.Data
{
    /// <summary>
    /// 数据表对象
    /// </summary>
    public class TableObject : SimpleModel
    {
        static ConcurrentDictionary<Type, PropertyInfo[]> typeProperties = new ConcurrentDictionary<Type, PropertyInfo[]>();



        /// <summary>
        /// 数据库连接上下文
        /// </summary>
        [ColumnMapping(Ignore = true)]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public DataContextBase Context { get; set; }

        /// <summary>
        /// 对象ID
        /// </summary>
        //[ColumnMapping(IndexType = PostgreSqlIndexType.BTREE, ColumnType = NpgsqlDbType.Text)]

        public string _id
        {
            get { return this.GetProperty<string>("_id"); }
            set { this.SetProperty("_id", value); }
        }
        /// <summary>
        /// 更新指定名称的字段
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<ResultInfo> UpdateFields(params string[] fields)
        {
            return await this.Context.UpdateTableObjectFieldsAsync(new List<TableObject> { this }, fields);
        }
        /// <summary>
        /// 保存当前对象
        /// </summary>
        /// <returns></returns>
        public async Task<ResultInfo> Save()
        {
            return await this.Context.UpdateTableObjectFieldsAsync(new List<TableObject> { this }, this.GetProperties().Keys.ToArray());

        }
        /// <summary>
        /// 上次更新时间
        /// </summary>
        public DateTime? UpdateTime
        {
            get { return this.GetProperty<DateTime?>("UpdateTime"); }
            set { this.SetProperty("UpdateTime", value); }
        }
        /// <summary>
        /// 检索指定属性是否存在
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public bool HasProperty(string propertyName)
        {
            return this.properties.ContainsKey(propertyName.ToLower());
        }


    }
}
