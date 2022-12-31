using ShaolanTech.Data.DBParameters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ShaolanTech.Data 
{
    public class ParameterCollection : IDisposable
    {
        internal DBParameterTypeMappingProvider provider = null;

        

        private int pIndex = 0;

        /// <summary>
        /// 参数总数量
        /// </summary>
        public int Count
        {
            get
            {
                return parameters.Count;
            }
        }
        List<DbParameter> parameters = new List<DbParameter>();


        private string prefix = "";
        /// <summary>
        /// 参数集合
        /// </summary>
        /// <param name="prefix">参数名前缀</param>
        public ParameterCollection(string prefix = ":")
        {
            this.prefix = prefix;
        }



        public ParameterCollection(IEnumerable<DbParameter> sourceParams)
        {
            if (sourceParams.Count() > 0)
            {
                var maxNumber = sourceParams.Select(p => p.ParameterName.Remove($"{prefix}p").TryParseInt()).Max() + 1;
                parameters.AddRange(sourceParams);
                pIndex = maxNumber;
            }

        }
        public static ParameterCollection From(IEnumerable<DbParameter> sourceParams)
        {
            return new ParameterCollection(sourceParams);
        }

        public void AddParameters(params object[] values)
        {
            foreach (var value in values)
            {
                AddParameter(value);
            }
        }
        /// <summary>
        /// 添加新参数
        /// </summary> 
        /// <param name="value">参数值</param>
        /// <param name="dbType">参数类型，默认字符串</param>
        /// <param name="parameterName">参数名</param>
        /// <returns></returns>
        public string AddParameter(object value, object defaultDbType=null, string parameterName = "")
        {

            var result = $"{prefix}p{pIndex}{parameterName}";
            var parameter = this.provider.MapObject(value, defaultDbType); 
            parameter.ParameterName = result; 
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }
            parameters.Add(parameter);
            pIndex++;

            return result;
        }


        /// <summary>
        /// 获取参数列表（List）
        /// </summary>
        /// <returns></returns>
        public List<DbParameter> ToList()
        {
            return parameters;
        }
        /// <summary>
        /// 获取参数列表（数组）
        /// </summary>
        /// <returns></returns>
        public DbParameter[] ToArray()
        {
            return parameters.ToArray();
        }
        /// <summary>
        /// 克隆新的参数集合
        /// </summary>
        /// <returns></returns>
        public DbParameter[] Clone()
        {
            List<DbParameter> result = new List<DbParameter>();
            foreach (var item in parameters)
            {
               
                result.Add(provider.CloneParameter(item));
            }
            return result.ToArray();
        }
        /// <summary>
        /// 清空参数集合
        /// </summary>
        public void Clear()
        {
            parameters.Clear();
            pIndex = 0;
        }
        /// <summary>
        /// 释放参数集合所占用资源
        /// </summary>
        public void Dispose()
        {
            parameters.Clear();
            parameters = null;
            //GC.Collect();
        }
    }



    public class ObjectParameterCollection : IDisposable
    {
        internal DBParameterTypeMappingProvider provider = null;
        private int pIndex = 1; 
        /// <summary>
        /// 参数总数量
        /// </summary>
        public int Count
        {
            get
            {
                return parameters.Count;
            }
        }
        List<DbParameter> parameters = new List<DbParameter>();


        private string prefix = "";
        /// <summary>
        /// 参数集合
        /// </summary>
        /// <param name="prefix">参数名前缀</param>
        public ObjectParameterCollection(string prefix = "$")
        {
            this.prefix = prefix;
        }
         

        public void AddParameters(params object[] values)
        {
            foreach (var value in values)
            {
                AddParameter(value);
            }
        }
        /// <summary>
        /// 添加新参数
        /// </summary> 
        /// <param name="value">参数值</param>
        /// <param name="dbType">参数类型，默认字符串</param>
        /// <param name="parameterName">参数名</param>
        /// <returns></returns>
        public string AddParameter(object value)
        {

            var result = $"{prefix}{pIndex}";
            var parameter = this.provider.CreateParameter(); 
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }
            parameters.Add(parameter);
            pIndex++;

            return result;
        }


        /// <summary>
        /// 获取参数列表（List）
        /// </summary>
        /// <returns></returns>
        public List<DbParameter> ToList()
        {
            return parameters;
        }
        /// <summary>
        /// 获取参数列表（数组）
        /// </summary>
        /// <returns></returns>
        public DbParameter[] ToArray()
        {
            return parameters.ToArray();
        }
        /// <summary>
        /// 克隆新的参数集合
        /// </summary>
        /// <returns></returns>
        public DbParameter[] Clone()
        {
            List<DbParameter> result = new List<DbParameter>();
            foreach (var item in parameters)
            {

                result.Add(provider.CloneParameter(item));
            }
            return result.ToArray();
        }
        /// <summary>
        /// 清空参数集合
        /// </summary>
        public void Clear()
        {
            parameters.Clear();
            pIndex = 0;
        }
        /// <summary>
        /// 释放参数集合所占用资源
        /// </summary>
        public void Dispose()
        {
            parameters.Clear();
            parameters = null;
            //GC.Collect();
        }
    }
}
