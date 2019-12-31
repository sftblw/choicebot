using System.Collections.Generic;
using System.Linq;
using Nadulmok.BotEvent;

namespace Nadulmok.BotApp
{
    public class AppBuilder
    {
        private List<BotEventHandler> _handlers = new List<BotEventHandler>();

        public void Use(BotEventHandler handler)
        {
            _handlers.Add(handler);
        }

        public App Build()
        {
            foreach(var handler in _handlers.AsEnumerable().Reverse())
            {
                
            }
        }
    }
}