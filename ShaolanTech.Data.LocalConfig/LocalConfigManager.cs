using LiteDB;

namespace ShaolanTech.Data.LocalConfig
{
    public static class LocalConfigManager
    {
        public static bool CollectionHasValue<T>(string key)
        {
            bool result = false;
            using (var context = new LiteDatabase("localconfig.db"))
            {
                var col = context.GetCollection<ConfigItem<T>>(typeof(T).Name);
                var config = col.FindById(new ObjectId(key));
                if (config != null)
                {
                    result = true;
                }
            }
            return result;
        }
        public static void SetCollection<T>(string key, T value)
        {
            using (var context = new LiteDatabase("localconfig.db"))
            {
                var col = context.GetCollection<ConfigItem<T>>(typeof(T).Name);

                var config = new ConfigItem<T>()
                {
                    _id = new ObjectId(key),
                    Value = value
                };
                col.Upsert(config);
            }
        }
        public static T GetCollection<T>(string key)
        {
            T result = default;
            using (var context = new LiteDatabase("localconfig.db"))
            {
                var col = context.GetCollection<ConfigItem<T>>(typeof(T).Name);
                var config = col.FindById(new ObjectId(key));
                if (config != null)
                {
                    result = (T)config.Value;
                }
            }
            return result;
        }
        public static void SetConfig(string key, object value)
        {
            using (var context=new LiteDatabase("localconfig.db"))
            {
                var col = context.GetCollection<ConfigItem>("Config");
                 
                var config = new ConfigItem() 
                {
                    _id=new ObjectId(key),
                    Value=value
                };
                col.Upsert(config);
            } 
        }

        public static T GetConfig<T>(string key)
        {
            T result = default;
            using (var context = new LiteDatabase("localconfig.db"))
            {
                var col = context.GetCollection<ConfigItem>("Config");
                var config=col.FindById(new ObjectId(key));
                if (config!=null)
                {
                    result = (T)config.Value;
                }
            }
            return result;
        }
    }
}
