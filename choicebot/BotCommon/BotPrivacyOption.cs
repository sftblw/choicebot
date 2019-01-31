using System;
using Mastonet;

namespace choicebot.ChoiceBotNS
{
    public class BotPrivacyOption
    {
        internal Visibility TargetVisibility { get; set; } = Visibility.Unlisted;
        internal BotVisibilityLimit VisibilityLimit { get; set; } = BotVisibilityLimit.LimitPublicLevel;

        internal bool PreserveContentWarning { get; set; } = false; // TODO. currently It's dummy

        internal Visibility ToBotVisibility(Visibility source)
        {
            switch(VisibilityLimit)
            {
                case BotVisibilityLimit.SameAsStatus:
                    return source;
                    
                case BotVisibilityLimit.LimitPublicLevel:
                    return (Visibility)Math.Max((int)TargetVisibility, (int)source);

                case BotVisibilityLimit.LimitPrivateLevel:
                    return (Visibility)Math.Min((int)TargetVisibility, (int)source);

                default: throw new InvalidOperationException($"No matched visibility. source: {source}, target: {TargetVisibility}, limit option: {VisibilityLimit}");
            }
        }
    }
}