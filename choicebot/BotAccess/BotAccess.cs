using Mastonet;
using Mastonet.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace choicebot
{
    public class BotAccess
    {
        [JsonProperty(propertyName: "app_registration", Required = Required.Always)]
        internal AppRegistration appRegistration;

        [JsonProperty(propertyName: "auth", Required = Required.Always)]
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
