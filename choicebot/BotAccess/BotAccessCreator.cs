using System;
using System.Threading.Tasks;
using Mastonet;
using Mastonet.Entities;

namespace ChoiceBot.BotAccess
{
    public static class BotAccessCreator
    {
        public static async Task<BotAccess> InteractiveConsoleRegister()
        {
            Console.WriteLine("Hello! You look like launching this app first time!");
            Console.WriteLine("I'll help you for authorizing your app.");
            Console.WriteLine();

            AppRegistration appReg =  await _InteractiveRegisterApp();
            var authClient = new AuthenticationClient(appReg);

            Console.WriteLine();

            Auth auth = await _InteractiveLogin(authClient);

            return new BotAccess(appReg, auth);
        }

        private static async Task<AppRegistration> _InteractiveRegisterApp()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("# Instance");
                    Console.Write("url: ");
                    string url = Console.ReadLine()?.Trim();

                    Console.Write("app name: ");
                    string appName = Console.ReadLine()?.Trim();

                    var authClient = new AuthenticationClient(url);
                    AppRegistration appRegistration = await authClient.CreateApp(appName, Scope.Read | Scope.Write | Scope.Follow);

                    return appRegistration;
                }
                catch (Exception ex)
                {
                    _ProcessUserInputError(ex);
                }
            }
        }

        private static async Task<Auth> _InteractiveLogin(IAuthenticationClient authClient)
        {
            bool isLoginByUrl;

            while (true)
            {
                try
                {
                    Console.WriteLine("Choose login method.");
                    Console.WriteLine("1: Login via URL (If you use 2-factor It's mandatory)");
                    Console.WriteLine("2: Login by ID, PW (not implemented yet)");
                    Console.WriteLine();
                    Console.Write("method : ");

                    int method = int.Parse(Console.ReadLine()?.Trim());
                    if (method != 1 && method != 2) { throw new InvalidOperationException("wrong item selected! 1 or 2 please."); }

                    isLoginByUrl = method == 1;

                    break;
                }
                catch (Exception ex)
                {
                    _ProcessUserInputError(ex);
                }
            }

            if (isLoginByUrl)
            {
                return await _InteractiveLoginUrl(authClient);
            }

            throw new NotImplementedException();
        }

        private static async Task<Auth> _InteractiveLoginUrl(IAuthenticationClient authClient)
        {
            string url = authClient.OAuthUrl();

            Console.WriteLine($"Login with this url : {url}");
            Console.Write("paste authorization token here :");

            string oauthCode = Console.ReadLine()?.Trim();
            Auth auth = await authClient.ConnectWithCode(oauthCode);

            return auth;
        }

        private static void _ProcessUserInputError(Exception ex)
        {
            Console.WriteLine("Something is wrong.");
            Console.WriteLine($"msg: {ex.Message}, type: {ex.GetType()}");
            Console.WriteLine("let's try again!");
        }
    }
}
