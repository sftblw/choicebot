using Mastonet;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using Mastonet.Entities;

namespace choicebot
{
    class Program
    {
        static void Main(string[] args)
        {
            Execute().Wait();
        }

        private async static Task Execute()
        {
            var mastoClient = await PrepareClient();

            await StartStreaming(mastoClient);
        }

        private static async Task StartStreaming(MastodonClient mastoClient)
        {
            Account botUserInfo = await mastoClient.GetCurrentUser();

            Console.WriteLine("choicebot running...");

            await new ChoiceBot(mastoClient).Start();
        }

        private async static Task<MastodonClient> PrepareClient()
        {
            const string clientPath = "./.config/configbotAccess.json";

            var persistent = new BotAccessPersistent(clientPath);

            MastodonClient client = (await persistent.Load())?.AsMastodonClient();

            if (client == null) {
                BotAccess access = await BotAccessCreator.InteractiveConsoleRegister();
                if (access != null) { await persistent.Save(access); }

                client = access?.AsMastodonClient();
            }

            if (client == null) { throw new Exception("client is null. somehow failed to create mastodon client."); }

            return client;
        }

        // not working and not needed but backup purposed
        //private static void WaitUntilExitPressed()
        //{
        //    ConsoleKeyInfo keyInfo;
        //    while (true)
        //    {
        //        keyInfo = Console.ReadKey();
        //        if (keyInfo.Key == ConsoleKey.C && keyInfo.Modifiers == ConsoleModifiers.Control)
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            Console.WriteLine("Hint: Press [Ctrl] + [C] to exit this app.");
        //        }
        //    }
        //}
    }
}
