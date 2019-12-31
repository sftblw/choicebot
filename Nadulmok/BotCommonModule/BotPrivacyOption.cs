using System;
using Nadulmok.BotCommon;
using Nadulmok.SocialApi;

namespace Nadulmok.BotCommonModule
{
    public class BotPrivacyOption
    {
        public Visibility TargetVisibility { get; set; } = Visibility.Unlisted;
        public BotVisibilityLimit VisibilityLimit { get; set; } = BotVisibilityLimit.LimitPublicLevel;

        public bool PreserveContentWarning { get; set; } = false; // TODO. currently It's dummy

        private Visibility ToBotVisibility(Visibility source)
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