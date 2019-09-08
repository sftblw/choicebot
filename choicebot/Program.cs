using Mastonet;
using System;
using System.Threading.Tasks;
using choicebot.BotAccess;
using choicebot.BotCommon;
using choicebot.ChoiceBotNS;

namespace choicebot
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

            await StartStreaming(mastoClient);
        }

        private static async Task StartStreaming(MastodonClient mastoClient)
        {
            Console.WriteLine("choicebot running...");

            var botManager = new BotManager(mastoClient);
            botManager.AddBot<ChoiceBot>();
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
