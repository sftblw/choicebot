using Mastonet;
using System;
using System.Threading.Tasks;
using choicebot_.BotAccess;
using choicebot_.BotCommon;
using choicebot_.ChoiceBotNS;

namespace choicebot_
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
            Console.WriteLine("choicebot_ running...");

            var botManager = new BotManager(mastoClient);
            botManager.AddBot<ChoiceBot>();
            await botManager.Start();
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
