using LiteDB;
using System;

namespace ShaolanTech.Data.LocalConfig
{
    public class ConfigItem
    {
        public ObjectId _id { get; set; }
      
        public object Value { get; set; }
    }

    public class ConfigItem<T>
    {
        public ObjectId _id { get; set; }

        public T Value { get; set; }
    }
}
