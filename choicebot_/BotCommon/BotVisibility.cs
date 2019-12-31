namespace choicebot_.ChoiceBotNS
{
    internal enum BotVisibilityLimit
    {
        SameAsStatus = 0,
        /// <summary>
        /// limit privacy level don't more opened than current setting.
        /// - e.g. setting: unlisted, input: public -> unlisted (threshold)
        /// </summary>
        LimitPublicLevel = 1,
        LimitPrivateLevel = 2
    }
}
