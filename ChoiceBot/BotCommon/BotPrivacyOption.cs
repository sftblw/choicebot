using System;
using Nadulmok.SocialApi;

namespace ChoiceBot.BotCommon
{
    public class BotPrivacyOption
    {
        internal StepVisibility TargetVisibility { get; set; } = StepVisibility.Unlisted;
        internal BotVisibilityLimit VisibilityLimit { get; set; } = BotVisibilityLimit.LimitPublicLevel;

        internal bool PreserveContentWarning { get; set; } = false; // TODO. currently It's dummy

        public ICommonVisibility ToBotVisibility(ICommonVisibility visibility)
        {
            visibility.Visibility = ToBotStepVisibility(visibility.Visibility);
            return visibility;
        }
        
        internal StepVisibility ToBotStepVisibility(StepVisibility source)
        {
            switch(VisibilityLimit)
            {
                case BotVisibilityLimit.SameAsStatus:
                    return source;
                    
                case BotVisibilityLimit.LimitPublicLevel:
                    return (StepVisibility)Math.Max((int)TargetVisibility, (int)source);

                case BotVisibilityLimit.LimitPrivateLevel:
                    return (StepVisibility)Math.Min((int)TargetVisibility, (int)source);

                default: throw new InvalidOperationException($"No matched visibility. source: {source}, target: {TargetVisibility}, limit option: {VisibilityLimit}");
            }
        }
    }
}