using System;
using System.Threading.Tasks;
using Nadulmok.SocialApi;

namespace ChoiceBot.BotCommon
{
    public delegate Task StatusProcessor(INote note, Func<Task> next);
}