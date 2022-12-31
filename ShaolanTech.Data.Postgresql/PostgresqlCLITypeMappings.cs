using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShaolanTech.Data.Postgresql
{
    /// <summary>
    /// Postgresql与CLI类型转化管理类
    /// </summary>
    public static class PostgresqlCLITypeMappings
    {
        /// <summary>
        /// CLI的Type与Postgresql数据类型枚举的对应表
        /// </summary>
        public static readonly Dictionary<Type, NpgsqlDbType> NpgSqlTypeMapping = new Dictionary<Type, NpgsqlDbType>()
        {
            { typeof (Enum),NpgsqlDbType.Integer},
            { typeof (int),NpgsqlDbType.Integer},
            { typeof (short),NpgsqlDbType.Smallint},
            { typeof (long),NpgsqlDbType.Bigint},
            { typeof (float),NpgsqlDbType.Real},
            { typeof (double),NpgsqlDbType.Double},
            { typeof (decimal),NpgsqlDbType.Money},
            { typeof (DateTime),NpgsqlDbType.Timestamp},
            { typeof (TimeSpan),NpgsqlDbType.Interval},
            { typeof (bool),NpgsqlDbType.Boolean},

             { typeof (int?),NpgsqlDbType.Integer},
            { typeof (short?),NpgsqlDbType.Smallint},
            { typeof (long?),NpgsqlDbType.Bigint},
            { typeof (float?),NpgsqlDbType.Real},
            { typeof (double?),NpgsqlDbType.Double},
            { typeof (decimal?),NpgsqlDbType.Money},
            { typeof (DateTime?),NpgsqlDbType.Timestamp},
            { typeof (TimeSpan?),NpgsqlDbType.Interval},
            { typeof (bool?),NpgsqlDbType.Boolean},



            { typeof (byte[]),NpgsqlDbType.Bytea},
            { typeof (Guid),NpgsqlDbType.Uuid},
            { typeof (string[]),NpgsqlDbType.Array| NpgsqlDbType.Text},
            { typeof (int[]),NpgsqlDbType.Array| NpgsqlDbType.Integer},
            { typeof (short[]),NpgsqlDbType.Array| NpgsqlDbType.Smallint},
            { typeof (long[]),NpgsqlDbType.Array| NpgsqlDbType.Bigint},
            { typeof (float[]),NpgsqlDbType.Array| NpgsqlDbType.Real},
            { typeof (double[]),NpgsqlDbType.Array| NpgsqlDbType.Double},
            { typeof (decimal[]),NpgsqlDbType.Array| NpgsqlDbType.Money},


            { typeof (List<string>),NpgsqlDbType.Array| NpgsqlDbType.Text},
            { typeof (List<int>),NpgsqlDbType.Array| NpgsqlDbType.Integer},
            { typeof (List<short>),NpgsqlDbType.Array| NpgsqlDbType.Smallint},
            { typeof (List<long>),NpgsqlDbType.Array| NpgsqlDbType.Bigint},
            { typeof (List<float>),NpgsqlDbType.Array| NpgsqlDbType.Real},
            { typeof (List<double>),NpgsqlDbType.Array| NpgsqlDbType.Double},
            { typeof (List<decimal>),NpgsqlDbType.Array| NpgsqlDbType.Money},



            { typeof (NpgsqlPoint),NpgsqlDbType.Point},
            { typeof (NpgsqlPoint[]),NpgsqlDbType.Array| NpgsqlDbType.Point},
            { typeof (NpgsqlLine),NpgsqlDbType.Line},
            { typeof (NpgsqlLine[]),NpgsqlDbType.Array| NpgsqlDbType.Line},
            { typeof (NpgsqlBox),NpgsqlDbType.Box},
            { typeof (NpgsqlBox[]),NpgsqlDbType.Array| NpgsqlDbType.Box},
            { typeof (NpgsqlPath),NpgsqlDbType.Path},
            { typeof (NpgsqlPath[]),NpgsqlDbType.Array| NpgsqlDbType.Path},
            { typeof (NpgsqlCircle),NpgsqlDbType.Circle},
            { typeof (NpgsqlCircle[]),NpgsqlDbType.Array| NpgsqlDbType.Circle},
            { typeof (NpgsqlPolygon),NpgsqlDbType.Polygon},
            { typeof (NpgsqlPolygon[]),NpgsqlDbType.Array| NpgsqlDbType.Polygon},
            { typeof (Dictionary<string,string>),NpgsqlDbType.Hstore}
        };
        /// <summary>
        /// Postgresql数据类型枚举与sql语句的对应表
        /// </summary>
        public static readonly Dictionary<NpgsqlDbType, string> NpgsqlTypeMappingNames = new Dictionary<NpgsqlDbType, string>()
        {
            { NpgsqlDbType.Text,"text"},
            { NpgsqlDbType.Xml,"xml"},
            { NpgsqlDbType.Jsonb,"jsonb"},
            { NpgsqlDbType.Integer,"Integer"},
            { NpgsqlDbType.Smallint,"Smallint"},
            {  NpgsqlDbType.Bigint,"Bigint"},
            { NpgsqlDbType.Real,"Real"},
            {  NpgsqlDbType.Double,"Double Precision"},
            { NpgsqlDbType.Money,"Money"},
            {  NpgsqlDbType.Timestamp,"Timestamp"},
            {  NpgsqlDbType.Interval,"Interval"},
            {  NpgsqlDbType.Boolean,"Boolean"},
            {  NpgsqlDbType.Bytea,"Bytea"},
            { NpgsqlDbType.Uuid,"Uuid"},
            {  NpgsqlDbType.Array| NpgsqlDbType.Text,"text[]"},
            {  NpgsqlDbType.Array| NpgsqlDbType.Integer,"Integer[]"},
            {  NpgsqlDbType.Array| NpgsqlDbType.Smallint,"Smallint[]"},
            {  NpgsqlDbType.Array| NpgsqlDbType.Bigint,"Bigint[]"},
            {  NpgsqlDbType.Array| NpgsqlDbType.Real,"Real[]"},
            {  NpgsqlDbType.Array| NpgsqlDbType.Double,"Double Precision[]"},

            {  NpgsqlDbType.Point,"Point"},
            {  NpgsqlDbType.Array| NpgsqlDbType.Point,"Point[]"},
            { NpgsqlDbType.Line,"Line"},
            { NpgsqlDbType.Array| NpgsqlDbType.Line,"Line[]"},
            {  NpgsqlDbType.Box,"Box"},
            {  NpgsqlDbType.Array| NpgsqlDbType.Box,"Box[]"},
            { NpgsqlDbType.Path,"Path"},
            {  NpgsqlDbType.Array| NpgsqlDbType.Path,"Path[]"},
            { NpgsqlDbType.Circle,"Circle"},
            {  NpgsqlDbType.Array| NpgsqlDbType.Circle,"Circle[]"},
            {  NpgsqlDbType.Polygon,"Polygon"},
            { NpgsqlDbType.Array| NpgsqlDbType.Polygon,"Polygon[]"},
            {  NpgsqlDbType.Hstore,"Hstore"},
            { NpgsqlDbType.TsVector,"tsvector"}
        };




        /// <summary>
        /// 创建Postgresql参数
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns></returns>
        public static NpgsqlParameter CreateParameter(object value)
        {
            NpgsqlParameter result = new NpgsqlParameter();
            if (value == null)
            {
                return result;
            }
            result.NpgsqlDbType = NpgsqlDbType.Text;//默认为字符串类型

            var type = value.GetType();
            if (NpgSqlTypeMapping.ContainsKey(type))
            {
                result.NpgsqlDbType = NpgSqlTypeMapping[type];
            }
            else
            {
                if (type == typeof(string))
                {
                    result.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
                }
                else
                {
                    result.NpgsqlDbType = NpgsqlDbType.Jsonb;
                }
            }


            return result;
        }
        /// <summary>
        /// 获取参数类型对的SQL类型字符串
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string ParamterSqlString(this NpgsqlParameter parameter)
        {
            if (NpgsqlTypeMappingNames.ContainsKey(parameter.NpgsqlDbType))
            {
                return NpgsqlTypeMappingNames[parameter.NpgsqlDbType];
            }
            else
            {
                return "text";
            }
        }
    }
}
