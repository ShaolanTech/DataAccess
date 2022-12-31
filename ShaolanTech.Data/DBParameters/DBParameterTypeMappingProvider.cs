using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ShaolanTech.Data.DBParameters
{
    /// <summary>
    /// Base abstract class for mapping objects to specified database types
    /// </summary>
    public abstract class DBParameterTypeMappingProvider
    {
        /// <summary>
        /// Map an object's CLR type to db type
        /// </summary>
        /// <param name="value">value to be mapped</param>
        /// <returns></returns>
        public abstract DbParameter MapObject(object value,object dbtype=null);

        /// <summary>
        /// Clone current parameter to a new one
        /// </summary>
        /// <param name="parameter">parameter to be cloned</param>
        /// <returns></returns>
        public abstract DbParameter CloneParameter(DbParameter parameter);

        public abstract DbParameter CreateParameter();
    }
}
