using Npgsql;
using ShaolanTech.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShaolanTech.Data.Postgresql
{
    public class CitusNodeAddress
    {
        public string IP { get; set; }
        public int Port { get; set; }

        
        public CitusNodeAddress(string ip, int port)
        {
            this.IP = ip;
            this.Port = port;
        }
    }
    public class CitusNodeServer
    {
        public PostgresqlDataContext Context { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public List<string> DocumentIDs { get; set; }
        public async Task<List<string>> GetTableNamesAsync(string prefix, string schema = "public", bool useInformationSchema = false)
        {
            return await Task.Run<List<string>>(() => GetTableNames(prefix, schema, useInformationSchema));
        }
        public List<string> GetTableNames(string prefix, string schema = "public", bool useInformationSchema = false)
        {
            if (useInformationSchema)
            {
                return this.Context.GetTableNames(prefix, schema);
            }
            else
            {
                var tables = this.Context.CreateDataSet("SELECT shard_name FROM citus_shards where shard_name~*(:p0) and nodename=:p1 and nodeport=:p2 order by shard_name", this.Context.NewParams($"{(schema == "public" ? "" : $"{schema}.")}{prefix}", this.IP, this.Port));
                List<string> result = new List<string>();
                foreach (var row in tables.ReadRow())
                {
                    result.Add(row.Read<string>("shard_name").Remove("\""));
                }
                return result;
            }

        }

    }
    public static class CitusCluster
    {

        public static void RunParallelOnWorkers(List<CitusNodeAddress> servers, int threadCount, Func<CitusNodeServer, Task> callback)
        {

            if (threadCount == 0)
            {
                threadCount = servers.Count;

            }

            servers.ParallelForeach(async server =>
            {
                NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();
                sb.Host = server.IP;
                sb.Port = server.Port;
                sb.Username = "postgres";
                sb.Password = "wbsbj@1108";
                sb.CommandTimeout = 3000;
                sb.Database = "TZC.DocumentPlatform";
                sb.Timeout = 300;
                sb.NoResetOnClose = true;
                sb.Pooling = false;
                sb.MinPoolSize = 0;
                sb.Encoding = "utf-8";
                var context = new CitusNodeServer() { Context = new PostgresqlDataContext(sb.ConnectionString), IP = server.IP, Port = server.Port };
                await callback(context);
            }, threadCount);
        }
        public static async Task RunParallelOnWorkersAsync(List<CitusNodeAddress> servers, int threadCount, Func<CitusNodeServer, Task> callback)
        {
            await Task.Run(()=> RunParallelOnWorkers(servers,threadCount,callback));
        }
    }
}
