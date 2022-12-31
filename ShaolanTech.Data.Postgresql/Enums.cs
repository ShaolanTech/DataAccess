using System;
using System.Collections.Generic;
using System.Text;

namespace ShaolanTech.Data.Postgresql
{
    /// <summary>
    /// PostgreSQL的索引类型
    /// </summary>
    public enum PostgreSqlIndexType
    {
        None,
        GIN,
        BTREE
    }
    public enum PostgreSqlIndexpOptions
    {
        None,
        /// <summary>
        /// 提供内置分词索引
        /// </summary>
        GIN_TRGM_Ops
    }
}
