using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShaolanTech.Data.Postgresql
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PostgresqlColumnMappingAttribute:ColumnMappingAttribute
    {

        /// <summary>
        /// 列类型
        /// </summary>
        public NpgsqlDbType ColumnType { get; set; } = NpgsqlDbType.Unknown;

        /// <summary>
        /// 索引类型
        /// </summary>
        public PostgreSqlIndexType IndexType { get; set; } = PostgreSqlIndexType.None;

        /// <summary>
        /// 特殊操作符
        /// </summary>
        public PostgreSqlIndexpOptions Options { get; set; } = PostgreSqlIndexpOptions.None;
    }
}
