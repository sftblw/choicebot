using System.Threading.Tasks;
using Nadulmok.BotEvent;

namespace Nadulmok.BotApp
{
    internal delegate Task BotPipe(BotEventHandler cur, BotEventHandler next);
}