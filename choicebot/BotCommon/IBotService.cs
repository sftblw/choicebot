using System.Collections.Generic;
using System.Threading.Tasks;
using Mastonet;

namespace choicebot.BotCommon
{
    public interface IBotService
    {
        MastodonClient MastoClient { get; set; }
        Task Initialize();
        IEnumerable<StatusProcessor> BuildPipeline();
    }
}