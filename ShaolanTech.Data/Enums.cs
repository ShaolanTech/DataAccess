using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaolanTech.Data
{
    /// <summary>
    /// 执行Upsert的冲突模式
    /// </summary>
    public enum UpsertConflictMode
    {
        /// <summary>
        /// 什么都不做
        /// </summary>
        DoNothing,
        /// <summary>
        /// 执行Update语句
        /// </summary>
        Update
    }
    public enum SQLDataType
    {
        String,
        IntNULL,
        DateTimeNULL,
        SingleNULL
    }
    public static class SQLAppendType
    {
        public const string And = " and ";
        public const string Or = " or ";
        public const string None = "  ";
    }
    public static class SQLOperator
    {
        public const string Equal = "=";
        public const string GreaterThan = ">";
        public const string LessThan = "<";
        public const string EqualOrGreaterThan = ">=";
        public const string EqualOrLessThan = "<=";
        public const string Like = "like";
    }
    /// <summary>
    /// 数据源类型
    /// </summary>
    public enum DataSourceType
    {
        /// <summary>
        /// 数据源为数据集
        /// </summary>
        DataSet,
        /// <summary>
        /// 数据源为字节流
        /// </summary>
        BytesBuffer
    }

    /// <summary>
    /// SQL查询类型
    /// </summary>
    public enum QueryType
    {
        /// <summary>
        /// 执行Update，Insert，Delete语句
        /// </summary>
        Modify,
        /// <summary>
        /// 执行Select语句，并返回数据集
        /// </summary>
        DataSet,
        /// <summary>
        /// 执行Select语句，并返回数据DataReader
        /// </summary>
        DataReader,
         /// <summary>
        /// 执行多个sql语句
        /// </summary>
        SQLTrans
    }
  
}
