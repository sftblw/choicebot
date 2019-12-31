using System.Collections.Generic;
using Nadulmok.SocialApi;

namespace Nadulmok.BotEvent
{
    public class BotEventContext
    {
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
        public Note ResponseNote { get; set; }
    }
}