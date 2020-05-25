using Mastonet;
using Mastonet.Entities;
using Newtonsoft.Json;

namespace ChoiceBot.BotAccess
{
    public class BotAccess
    {
        [JsonProperty("app_registration", Required = Required.Always)]
        internal AppRegistration appRegistration;

        [JsonProperty("auth", Required = Required.Always)]
        internal Auth botAuth;

        public BotAccess(AppRegistration appRegistration, Auth botAuth)
        {
            this.appRegistration = appRegistration;
            this.botAuth = botAuth;
        }

        public MastodonClient AsMastodonClient()
        {
            return new MastodonClient(appRegistration, botAuth);
        }
    }
}
