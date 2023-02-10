using LiteDB;
using System;

namespace ShaolanTech.Data.LocalConfig
{
    public class ConfigItem
    {
        public ObjectId Id { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
    }

    public class ConfigItem<T>
    {
        public ObjectId Id { get; set; }
        public string Key { get; set; }
        public T Value { get; set; }
    }
}
