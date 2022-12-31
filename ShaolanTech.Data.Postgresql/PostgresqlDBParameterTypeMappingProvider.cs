using Npgsql;
using NpgsqlTypes;
using ShaolanTech.Data.DBParameters;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ShaolanTech.Data.Postgresql
{
    public class PostgresqlDBParameterTypeMappingProvider : DBParameterTypeMappingProvider
    {
        public override DbParameter CloneParameter(DbParameter parameter)
        {
            var result = new NpgsqlParameter();
            var old = (NpgsqlParameter)parameter;
            result.NpgsqlDbType = old.NpgsqlDbType;
            result.NpgsqlValue = old.NpgsqlValue;
            result.ParameterName = old.ParameterName;
            return result;
        }

        public override DbParameter CreateParameter()
        {
            return new NpgsqlParameter();
        }

        public override DbParameter MapObject(object value, object dbtype = null)
        {
            var result=DBTypeMappings.CreateParameter(value);
            if (dbtype!=null)
            {
                result.NpgsqlDbType = (NpgsqlDbType)dbtype;
            }
            return result;
        }
    }
}
