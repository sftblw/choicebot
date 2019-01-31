using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace choicebot
{
    public class BotAccessPersistent
    {
        private readonly string path = null;

        public BotAccessPersistent(string persistentPath)
        {
            path = persistentPath;
            if (Path.GetFileName(path) == null) { throw new ArgumentException("path is invalid"); }
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.Create(path).Dispose();
            }
        }

        public async Task<BotAccess> Load()
        {
            if (File.Exists(path))
            {
                var jsonSettings = new JsonSerializerSettings()
                {
                    ContractResolver = new IgnoreJsonAttributesResolver()
                };



                string botConfigText;
                using (var reader = File.OpenText(path))
                {
                    botConfigText = await reader.ReadToEndAsync();
                }

                var botAccess = JsonConvert.DeserializeObject<BotAccess>(File.ReadAllText(path), jsonSettings);

                return botAccess;
            }

            return null;
        }

        public async Task Save(BotAccess accessData)
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new IgnoreJsonAttributesResolver()
            };

            using (var writer = File.CreateText(path))
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(accessData, jsonSettings));
            }
        }
    }

    // https://stackoverflow.com/a/37376089/4394750
    class IgnoreJsonAttributesResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            foreach (var prop in props)
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
