using System.Collections.Generic;
using System.Threading.Tasks;
using ChoiceBot.SocialApi;
using Mastonet;

namespace ChoiceBot.BotCommon
{
    public interface IBotService
    {
        Task Initialize();
        IEnumerable<StatusProcessor> BuildPipeline();
    }
}