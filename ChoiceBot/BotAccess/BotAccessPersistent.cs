using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ChoiceBot.BotAccess
{
    public class BotAccessPersistent
    {
        private readonly string _path;

        public BotAccessPersistent(string persistentPath)
        {
            _path = persistentPath;
            if (Path.GetFileName(_path) == null) { throw new ArgumentException("path is invalid"); }

            if (File.Exists(_path)) return;
            Directory.CreateDirectory(Path.GetDirectoryName(_path));
            File.Create(_path).Dispose();
        }

        public async Task<BotAccess> Load()
        {
            if (!File.Exists(_path)) { return null; }
            
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new IgnoreJsonAttributesResolver()
            };

            var botAccess = JsonConvert.DeserializeObject<BotAccess>(await File.ReadAllTextAsync(_path), jsonSettings);

            return botAccess;

        }

        public async Task Save(BotAccess accessData)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new IgnoreJsonAttributesResolver()
            };

            await File.WriteAllTextAsync(_path, JsonConvert.SerializeObject(accessData, jsonSettings));
        }
    }

    // https://stackoverflow.com/a/37376089/4394750
    public class IgnoreJsonAttributesResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            foreach (JsonProperty prop in props)
            {
                // Only ignore [JsonIgnore]
                prop.Ignored = false;   // Ignore [JsonIgnore]

                //prop.Converter = null;  // Ignore [JsonConverter]
                //prop.PropertyName = prop.UnderlyingName;  // restore original property name
            }
            return props;
        }
    }
}
