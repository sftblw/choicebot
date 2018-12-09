using System;
using System.Threading.Tasks;
using Mastonet.Entities;

namespace choicebot.BotCommon
{
    public delegate Task StatusProcessor(Status status, Func<Task> next);
}