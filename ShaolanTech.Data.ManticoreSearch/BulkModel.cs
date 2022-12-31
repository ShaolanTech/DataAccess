using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ShaolanTech.Data.ManticoreSearch
{
    public class ManticoreBulkModel
    {
        public BulkOperations Operation { get; set; } = BulkOperations.Replace;
        public long DocID { get; set; }
        public string TableName { get; set; }

        public object Doc { get; set; }

        public string ToHttpJson()
        {
            var dic = new Dictionary<string, object>
            {
                {
                    this.Operation.ToString().ToLower(),
                    new Dictionary<string, object>
                    {
                        { "index",this.TableName},
                        { "id",this.DocID},
                        { "doc",this.Doc}
                    }
                }
            };
            return dic.ToJsonString();
        }
    }
}
