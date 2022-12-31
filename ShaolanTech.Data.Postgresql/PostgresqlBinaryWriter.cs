using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShaolanTech.Data.Postgresql
{
    public class PostgresqlBinaryWriter : IDisposable
    {
        int rowIndex = 0;
        public static bool WithNameSpace { get; set; } = true;
        private bool replace = false;
        private bool commited = false;
        private string table;
        private string[] fields;
        private NpgsqlBinaryImporter writer;
        private NpgsqlConnection connection;
        /// <summary>
        /// 是否自动提交，默认False
        /// </summary>
        public bool AutoCommit { get; set; } = true;
        public bool AllowNullString { get; set; } = true;
        private bool inTransaction = false;
        public PostgresqlBinaryWriter(NpgsqlConnection con, string table, params string[] fields)
        {
            try
            {
                this.connection = con;
                this.connection.Open();
                this.table = table;
                this.fields = fields;
                this.writer = ((NpgsqlConnection)this.connection).BeginBinaryImport($"COPY {table} ({string.Join(",", fields)}) FROM STDIN (FORMAT BINARY)");
            }
            catch (Exception ex)
            {


            }

        }
        public PostgresqlBinaryWriter(NpgsqlConnection con, bool replace, string table, params string[] fields)
        {
            try
            {
                this.replace = replace;
                this.connection = con;
                this.connection.Open();
                this.table = table;
                this.fields = fields;
                this.writer = ((NpgsqlConnection)this.connection).BeginBinaryImport($"COPY {table} ({string.Join(",", fields)}) FROM STDIN (FORMAT BINARY{(replace ? ",replace" : "")})");
            }
            catch (Exception ex)
            {


            }

        }
        public PostgresqlBinaryWriter(NpgsqlConnection con, string table, bool inTransaction, params string[] fields)
        {
            this.inTransaction = inTransaction;
            if (!inTransaction)
            {
                this.connection = con;
                this.connection.Open();
            }
            this.connection = con;
            this.table = table;
            this.fields = fields;
            this.writer = ((NpgsqlConnection)this.connection).BeginBinaryImport($"COPY {table} ({string.Join(",", fields)}) FROM STDIN (FORMAT BINARY)");
        }
        public int AutoCommitCount { get; set; } = 0;
        public PostgresqlBinaryWriter StartRow()
        {
            this.rowIndex++;
            if (this.AutoCommitCount > 0 && this.rowIndex >= this.AutoCommitCount && AutoCommit)
            {
                this.writer.Complete();
                this.writer.Dispose();
                this.writer = ((NpgsqlConnection)this.connection).BeginBinaryImport($"COPY {table} ({string.Join(",", fields)}) FROM STDIN (FORMAT BINARY {(replace ? ",replace" : "")})  ");
                this.commited = true;
                this.rowIndex = 0;
            }
            else
            {
                this.commited = false;
            }
            this.writer.StartRow();
            return this;
        }
        /// <summary>
        /// 写入对象
        /// </summary>
        /// <param name="obj">对象</param>
        public PostgresqlBinaryWriter Write<T>(T data)
        {

            this.writer.Write(data);

            return this;
        }
        public PostgresqlBinaryWriter Write(string data)
        {
            if (this.AllowNullString)
            {
                this.writer.Write(data);
            }
            else
            {
                this.writer.Write(data.IsNullOrEmpty() ? "" : data);
            }
            return this;
        }
        public PostgresqlBinaryWriter Write<T>(T data, NpgsqlDbType type)
        {
            this.writer.Write(data, type);
            return this;
        }
        public void Commit()
        {
            this.writer.Complete();
        }
        public void Dispose()
        {
            if (this.AutoCommit)
            {
                try
                {
                    this.writer.Complete();
                }
                catch (Exception ex)
                {


                }

            }

            this.writer.Dispose();
            if (!this.inTransaction)
            {
                this.connection.Dispose();
            }

        }
    }
}
