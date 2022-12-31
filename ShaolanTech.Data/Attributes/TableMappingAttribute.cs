using System;
using System.Collections.Generic;
using System.Text;

namespace ShaolanTech.Data
{
    public class TableMappingAttribute : Attribute
    {
        /// <summary>
        /// 映射到的表名
        /// </summary>
        public string MappingTableName { get; set; }
        public string Schema { get; set; } = "public";
        /// <summary>
        /// 表备注
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 在创建表时是否忽略本对象
        /// </summary>
        public bool Ignore { get; set; } = false;

        public TableMappingAttribute()
        { }

    }
}
