using MySqlConnector;
using ShaolanTech.Data.DBParameters;
using System;
using System.Data.Common;

namespace ShaolanTech.Data.ManticoreSearch
{
    public class ManticoreDBParameterTypeMappingProvider : DBParameterTypeMappingProvider
    {
        public override DbParameter CloneParameter(DbParameter parameter)
        {
            throw new NotImplementedException();
        }

        public override DbParameter CreateParameter()
        {
            return new MySqlParameter();
        }

        public override DbParameter MapObject(object value, object dbtype = null)
        {
            var result = DBTypeMappings.CreateParameter(value);
            if (dbtype != null)
            {
                result.MySqlDbType = (MySqlDbType)dbtype;
            }
            return result;
        }
    }
}
