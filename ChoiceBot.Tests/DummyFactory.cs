using ChoiceBot.BotCommon;
using Nadulmok.SocialApi.Dummy;

namespace ChoiceBot.Tests
{
    public class DummyFactory
    {
        public static DummyApiClient CreateDummyApiClient()
        {
            var apiClient = new DummyApiClient
            {
                CurrentUser = {Id = "123", DisplayName = "The Choicer Bot", IsBot = true, UserName = "choicebot"}
            };

            return apiClient;
        }
        
        public static BotManager CreateDummyChoiceBot()
        {
            var apiClient = CreateDummyApiClient();
            var botManager = new BotManager(apiClient);
            botManager.AddBot(new ChoiceBotMain.ChoiceBot(apiClient));
            
            return botManager;
        }
    }
}