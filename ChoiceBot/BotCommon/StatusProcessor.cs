using System;
using System.Threading.Tasks;
using ChoiceBot.SocialApi;

namespace ChoiceBot.BotCommon
{
    public delegate Task StatusProcessor(INote note, Func<Task> next);
}