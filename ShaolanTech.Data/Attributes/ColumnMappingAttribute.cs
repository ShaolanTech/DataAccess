using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ShaolanTech.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ColumnMappingAttribute : Attribute
    {
        /// <summary>
        /// 是否忽略本字段
        /// </summary>
        public bool Ignore { get; set; } = false;
        /// <summary>
        /// 列备注
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 是否为列名添加双引号（如果列表为if,is）
        /// </summary>
        public bool Quotes { get; set; } = false;

        
    }
}
