using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace choicebot.BotAccess
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
            
            string botConfigText;
            using (var reader = File.OpenText(_path))
            {
                botConfigText = await reader.ReadToEndAsync();
            }

            var botAccess = JsonConvert.DeserializeObject<BotAccess>(botConfigText, jsonSettings);

            return botAccess;

        }

        public async Task Save(BotAccess accessData)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new IgnoreJsonAttributesResolver()
            };

            using (var writer = File.CreateText(_path))
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(accessData, jsonSettings));
            }
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
