using Mastonet;
using System;
using System.Threading.Tasks;
using ChoiceBot.BotAccess;
using ChoiceBot.BotCommon;
using Nadulmok.SocialApi.MastoNet;

namespace ChoiceBot
{
    public static class Program
    {
        private static MastodonClient _client;
        private const string ExceptionMessage = "[!] 예외가 발생하였습니다.\r\n@sftblw@twingyeo.kr";

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Execute().Wait();
        }

        private static async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.Error.WriteLine(e);
            await _client.PostStatus(ExceptionMessage, Visibility.Unlisted);
        }

        private static async Task Execute()
        {
            MastodonClient mastoClient = await PrepareClient();
            _client = mastoClient;

            await Start(mastoClient);
        }

        private static async Task Start(MastodonClient mastoClient)
        {
            var client = mastoClient.ToCommon();
            
            var botManager = new BotManager(client);
            botManager.AddBot(new ChoiceBotMain.ChoiceBot(client));
            
            Console.WriteLine("choicebot running...");
            await botManager.Start();
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
