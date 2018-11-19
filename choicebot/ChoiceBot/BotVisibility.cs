using Mastonet;
using System;

namespace choicebot
{
    enum BotVisibilityLimit
    {
        SameAsStatus = 0,
        /// <summary>
        /// limit privacy level don't more opened than current setting.
        /// - e.g. setting: unlisted, input: public -> unlisted (threshold)
        /// </summary>
        LimitOpenness = 1,
        LimitClosedness = 2
    }

    internal class BotPrivacyOption
    {
        internal Visibility TargetVisibility { get; set; } = Visibility.Unlisted;
        internal BotVisibilityLimit VisibilityLimit { get; set; } = BotVisibilityLimit.LimitOpenness;

        internal bool PreserveContentWarning { get; set; } = false; // TODO. currently It's dummy

        internal Visibility ToBotVisibility(Visibility source)
        {
            switch(VisibilityLimit)
            {
                case BotVisibilityLimit.SameAsStatus:
                    return source;
                    
                case BotVisibilityLimit.LimitOpenness:
                    return (Visibility)Math.Max((int)this.TargetVisibility, (int)source);

                case BotVisibilityLimit.LimitClosedness:
                    return (Visibility)Math.Min((int)this.TargetVisibility, (int)source);

                default: throw new InvalidOperationException($"No matched visibility. source: {source}, target: {TargetVisibility}, limit option: {VisibilityLimit}");
            }
        }
    }
}
