using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ShaolanTech.Data.Postgresql
{
    public class PostgresqlExecutePlan : IDisposable
    {
        private NpgsqlConnection connection;
        private NpgsqlCommand command = null;
        private NpgsqlDataAdapter adpt = null;

        public PostgresqlExecutePlan(NpgsqlConnection connection, NpgsqlCommand cmd)
        {
            this.connection = connection;
            this.command = cmd;
            this.adpt = new NpgsqlDataAdapter(cmd);
        }

        public ResultInfo<DataSet> CreateDataSet(params object[] parameters)
        {
            ResultInfo<DataSet> result = new ResultInfo<DataSet>();
            try
            {
                for (int i = 0; i < this.command.Parameters.Count; i++)
                {
                    this.command.Parameters[i].Value = parameters[i];
                }
                var dataset = new DataSet();
                this.adpt.Fill(dataset);
                result.AdditionalData = dataset;
            }
            catch (Exception ex)
            {
                result.OperationDone = false;
                result.Message = ex.Message;

            }
            return result;
        }
        /// <summary>
        /// 执行已经准备好的SQL语句 
        /// </summary>
        /// <param name="parameters">参数值列表</param>
        /// <returns></returns>
        public async Task<ResultInfo<int>> Execute(params object[] parameters)
        {
            ResultInfo<int> result = new ResultInfo<int>();
            try
            {
                for (int i = 0; i < this.command.Parameters.Count; i++)
                {
                    this.command.Parameters[i].Value = parameters[i];
                }
                var count = await this.command.ExecuteNonQueryAsync();
                result.AdditionalData = count;
            }
            catch (Exception ex)
            {
                result.OperationDone = false;
                result.Message = ex.Message;

            }
            return result;
        }
        public void Dispose()
        {
            this.command.Dispose();
            this.connection.Dispose();
        }
    }
}
