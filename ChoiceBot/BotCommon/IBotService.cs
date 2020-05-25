using System.Collections.Generic;
using System.Threading.Tasks;
using Mastonet;

namespace ChoiceBot.BotCommon
{
    public interface IBotService
    {
        Task Initialize();
        IEnumerable<StatusProcessor> BuildPipeline();
    }
}