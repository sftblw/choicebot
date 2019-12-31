using System;
using System.Threading.Tasks;
using ChoiceBot.BotAccess;
using Mastonet;

namespace ChoiceBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Execute().Wait();
        }

        private static async Task Execute()
        {
            MastodonClient mastoClient = await PrepareClient();
            
        }

        private static async Task<MastodonClient> PrepareClient()
        {
            const string clientPath = "./.config/botAccessConfig.json";

            var persistent = new BotAccessPersistent(clientPath);

            MastodonClient preparedClient = (await persistent.Load())?.AsMastodonClient();

            if (preparedClient == null) {
                BotAccess.BotAccess access = await BotAccessCreator.InteractiveConsoleRegister();
                if (access != null) { await persistent.Save(access); }

                preparedClient = access?.AsMastodonClient();
            }

            if (preparedClient == null) { throw new Exception("client is null. somehow failed to create mastodon client."); }

            return preparedClient;
        }
    }
}