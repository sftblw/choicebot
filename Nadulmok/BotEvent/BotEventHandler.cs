using System.Threading.Tasks;

namespace Nadulmok.BotEvent
{
    public delegate Task BotEventHandler(BotEventContext context, BotEventHandler next);
}