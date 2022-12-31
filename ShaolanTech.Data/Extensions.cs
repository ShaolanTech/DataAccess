using System;
using System.Data.Common;

namespace ShaolanTech.Data
{
    /// <summary>
    /// 数据访问层扩展类
    /// </summary>
    public static class Extensions
    {



        public static  T Read<T>(this DbDataReader reader,int index)
        {
            var obj = reader[index];
            if (obj.Equals(DBNull.Value))
            {
                return default(T);
            }
            else
            {
                return (T)obj;
            }
        }
        public static T ReadJson<T>(this DbDataReader reader,string column)
        {
            return reader.GetFieldValue<T>(reader.GetOrdinal(column));
        }
        public static T ReadJson<T>(this DbDataReader reader,int column)
        {
            return reader.GetFieldValue<T>(column);
        }
        public static T Read<T>(this DbDataReader reader,string column)
        {
            var obj = reader[column];
            if (obj.Equals(DBNull.Value))
            {
                return default(T);
            }
            else
            {
                return (T)obj;
            }
        }

    }
}
