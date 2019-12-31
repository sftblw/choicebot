using System;

namespace Nadulmok.SocialApi
{
    public class Note
    {
        public string ReplyToId { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        [Obsolete("use summary")]
        public string ContentWarning { get => Summary; set => Summary = value; }
    }
}