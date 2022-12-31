using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ShaolanTech.Data.Postgresql
{
    /// <summary>
    /// 运行时TableObject的属性全局缓存
    /// </summary>
    public static class DBTypeMappings
    {
        private static ConcurrentDictionary<Type, PropertyInfo[]> runtimeProperties = new ConcurrentDictionary<Type, PropertyInfo[]>();
        /// <summary>
        /// 准备类型
        /// </summary>
        /// <param name="type">表类型</param>
        public static void PrepareType(Type type)
        {
            if (runtimeProperties.ContainsKey(type) == false)
            {
                var properties = type.GetRuntimeProperties();
                runtimeProperties.TryAdd(type, properties.Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null).Where(p => p.GetCustomAttribute<ColumnMappingAttribute>() == null || (p.GetCustomAttribute<ColumnMappingAttribute>() != null && p.GetCustomAttribute<ColumnMappingAttribute>().Ignore == false)).ToArray());
            }
        }
        /// <summary>
        /// 准备类型
        /// </summary>
        public static void PrepareType<T>() where T : TableObject
        {
            PrepareType(typeof(T));
        }
        /// <summary>
        /// 获取运行时属性
        /// </summary>
        /// <param name="type">表类型</param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            if (runtimeProperties.ContainsKey(type) == false)
            {
                PrepareType(type);
            }
            return runtimeProperties[type];
        }
        /// <summary>
        /// 获取运行时属性
        /// </summary>
        public static PropertyInfo[] GetProperties<T>() where T : TableObject
        {
            return GetProperties(typeof(T));
        }

        /// <summary>
        /// 创建Postgresql参数
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns></returns>
        public static NpgsqlParameter CreateParameter(object value)
        {
            return PostgresqlCLITypeMappings.CreateParameter(value);
        }
        /// <summary>
        /// 创建Postgresql参数
        /// </summary>
        /// <param name="type">表类型</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static NpgsqlParameter CreateParameter(Type type, string propertyName)
        {
            NpgsqlParameter result = new NpgsqlParameter();
            result.NpgsqlDbType = NpgsqlDbType.Text;//默认为字符串类型
            var properties = GetProperties(type);
            var property = properties.FirstOrDefault(p => p.Name.ToLower() == propertyName.ToLower());
            //如果标注了ColumnMappingAttribute，则按使用都标注的类型算
            var attr = property.GetCustomAttribute<PostgresqlColumnMappingAttribute>();
            if (attr != null)
            {
                result.NpgsqlDbType = attr.ColumnType;
            }
            else
            {
                if (PostgresqlCLITypeMappings.NpgSqlTypeMapping.ContainsKey(property.PropertyType))
                {
                    result.NpgsqlDbType = PostgresqlCLITypeMappings.NpgSqlTypeMapping[property.PropertyType];
                }
                else
                {
                    if (property.PropertyType == typeof(string))
                    {
                        result.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
                    }
                    else
                    {
                        result.NpgsqlDbType = NpgsqlDbType.Jsonb;
                    }
                }
            }

            return result;
        }



    }
}
