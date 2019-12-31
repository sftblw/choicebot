using System.Threading.Tasks;
using Nadulmok.BotEvent;
using Nadulmok.SocialApi;

namespace Nadulmok.BotApp
{
    public class App
    {
        private readonly IApiClient _client;
        private readonly BotEventHandler _entryHandler;
        
        public App(IApiClient client, BotEventHandler entryHandler)
        {
            _client = client;
            _entryHandler = entryHandler;
        }

        public Task Execute()
        {
            
        }
    }
}