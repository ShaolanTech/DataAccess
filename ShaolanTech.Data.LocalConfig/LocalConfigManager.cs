using LiteDB;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ShaolanTech.Data.LocalConfig
{

    public static class LocalConfigManager
    {
        public static string ConfigRoot { get; set; } = "";

        private static string DBFile
        {
            get
            {
                if (ConfigRoot == "")
                {
                    return "localconfig.db";
                }
                else
                {
                    if (ConfigRoot.EndsWith("/"))
                    {
                        return $"{ConfigRoot}localconfig.db";
                    }
                    else
                    {
                        return $"{ConfigRoot}/localconfig.db";
                    }
                }
            }
        }
        private static LiteDatabase instance = null;
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static LiteDatabase GetContext()
        {
            if (instance == null)
            {
                instance = new LiteDatabase(DBFile);
            }
            return instance;
        }
        public static bool CollectionHasValue<T>(string key)
        {
            bool result = false;
            var context = GetContext();

            var col = context.GetCollection<ConfigItem<T>>(typeof(T).Name);
            var config = col.FindOne(d => d.Key == key);
            if (config != null)
            {
                result = true;
            }

            return result;
        }
        public static void SetCollection<T>(string key, T value)
        {
            var context = GetContext();
            var col = context.GetCollection<ConfigItem<T>>(typeof(T).Name);

            var old = col.FindOne(d => d.Key == key);
            if (old == null)
            {
                var config = new ConfigItem<T>()
                {
                    Key = key,
                    Value = value
                };
                col.Insert(config);
            }
            else
            {
                old.Value = value;
                col.Update(old);
            }

        }
        public static T GetCollection<T>(string key)
        {
            T result = default;
            var context = GetContext();

            var col = context.GetCollection<ConfigItem<T>>(typeof(T).Name);
            var config = col.FindOne(d => d.Key == key);
            if (config != null)
            {
                result = (T)config.Value;
            }

            return result;
        }
        public static void SetConfig(string key, object value)
        {
            var context = GetContext();

            var col = context.GetCollection<ConfigItem>("Config");
            var old = col.FindOne(d => d.Key == key);
            if (old == null)
            {
                var config = new ConfigItem()
                {
                    Key = key,
                    Value = JsonConvert.SerializeObject(value)
                };
                col.Insert(config);
            }
            else
            {
                old.Value = value;
                col.Update(old);
            }

        }

        public static T GetConfig<T>(string key)
        {
            T result = default;
            var context = GetContext();
            var col = context.GetCollection<ConfigItem>("Config");
            var config = col.FindOne(d => d.Key == key);
            if (config != null)
            {
                result = JsonConvert.DeserializeObject<T>((string)config.Value);
            }

            return result;
        }
    }
}
