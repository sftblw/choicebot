namespace ChoiceBot.SocialApi.Streaming
{
    public interface INoti
    {
        /// <summary>
        /// If notification type is not known to this lib,
        /// It will not cause error and will return <see cref="NotiType.Unknown"/>
        /// </summary>
        public NotiType Type { get; }
        public IAccount Account { get; }
        public INote? Note { get; }
    }
}