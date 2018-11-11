using Mastonet;
using Mastonet.Entities;
using System;
using System.Threading.Tasks;

namespace choicebot
{
    class BotAccessCreator
    {
        public static async Task<BotAccess> InteractiveConsoleRegister()
        {
            Console.WriteLine("Hello! You look like launching this app first time!");
            Console.WriteLine("I'll help you for authorizing your app.");
            Console.WriteLine();

            var appReg =  await InteractiveRegisterApp();
            var authClient = new AuthenticationClient(appReg);

            Console.WriteLine();

            var auth = await InteractiveLogin(authClient);

            return new BotAccess(appReg, auth);
        }

        public static async Task<Mastonet.Entities.AppRegistration> InteractiveRegisterApp()
        {
            AuthenticationClient authClient = null;
            AppRegistration appRegistration = null;

            while (true)
            {
                try
                {
                    Console.WriteLine("# Instance");
                    Console.Write("url: ");
                    var url = Console.ReadLine().Trim();

                    Console.Write("app name: ");
                    var appName = Console.ReadLine().Trim();

                    authClient = new AuthenticationClient(url);
                    appRegistration = await authClient.CreateApp(appName, Scope.Read | Scope.Write | Scope.Follow);

                    return appRegistration;
                }
                catch (Exception ex)
                {
                    ProcessUserInputError(ex);
                }
            }
        }

        public static async Task<Mastonet.Entities.Auth> InteractiveLogin(AuthenticationClient authClient)
        {
            bool isLoginByURL = true;

            while (true)
            {
                try
                {
                    Console.WriteLine("Choose login method.");
                    Console.WriteLine("1: Login via URL (If you use 2-factor It's mandatory)");
                    Console.WriteLine("2: Login by ID, PW");
                    Console.WriteLine();
                    Console.Write("method : ");

                    var method = int.Parse(Console.ReadLine().Trim());
                    if (method != 1 && method != 2) { throw new InvalidOperationException("wrong item selected! 1 or 2 please."); }

                    isLoginByURL = method == 1;

                    break;
                }
                catch (Exception ex)
                {
                    ProcessUserInputError(ex);
                }
            }

            if (isLoginByURL)
            {
                var url = authClient.OAuthUrl();

                Console.WriteLine($"Login with this url : {url}");
                Console.Write("paste authorization token here :");

                var oauthCode = Console.ReadLine().Trim();
                var auth = await authClient.ConnectWithCode(oauthCode);

                return auth;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void ProcessUserInputError(Exception ex)
        {
            Console.WriteLine("Something is wrong.");
            Console.WriteLine($"msg: {ex.Message}, type: {ex.GetType()}");
            Console.WriteLine("let's try again!");
        }
    }
}
