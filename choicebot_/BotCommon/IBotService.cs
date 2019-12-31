using System.Collections.Generic;
using System.Threading.Tasks;
using Mastonet;

namespace choicebot_.BotCommon
{
    public interface IBotService
    {
        MastodonClient MastoClient { get; set; }
        Task Initialize();
        IEnumerable<StatusProcessor> BuildPipeline();
    }
}