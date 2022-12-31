using MySqlConnector;
using System;
using System.Collections.Generic;

namespace ShaolanTech.Data.ManticoreSearch
{
    internal class DBTypeMappings
    {
        /// <summary>
        /// CLI的Type与Manticore数据类型枚举的对应表
        /// </summary>
        public static readonly Dictionary<Type, MySqlDbType> MySqlTypeMapping = new Dictionary<Type, MySqlDbType>()
        {

            { typeof (int),MySqlDbType.Int32},
            { typeof (long),MySqlDbType.Int64},
            { typeof (float),MySqlDbType.Float},
            { typeof (DateTime),MySqlDbType.Timestamp},
            { typeof (bool),MySqlDbType.Int16},
            { typeof (int?),MySqlDbType.Int32},
            { typeof (long?),MySqlDbType.Int64},
            { typeof (float?),MySqlDbType.Float},
            { typeof (DateTime?),MySqlDbType.Timestamp},
            { typeof (bool?),MySqlDbType.Int16},


        };


        /// <summary>
        /// 创建Postgresql参数
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns></returns>
        public static MySqlParameter CreateParameter(object value)
        {


            MySqlParameter result = new MySqlParameter();
            if (value == null)
            {
                return result;
            }
            result.MySqlDbType = MySqlDbType.String;

            var type = value.GetType();
            if (MySqlTypeMapping.ContainsKey(type))
            {
                result.MySqlDbType = MySqlTypeMapping[type];
            }
            else
            {
                if (type == typeof(string))
                {
                    result.MySqlDbType = MySqlDbType.String;
                }
                else
                {
                    result.MySqlDbType = MySqlDbType.JSON;
                }
            }


            return result;
        }
    }
}
