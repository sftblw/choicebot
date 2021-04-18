namespace Nadulmok.SocialApi
{
    public interface IAccount
    {
        /** Internal ID. assume It as UUID. on Mastodon, It's int but this library doesn't treat it as Int */
        string Id { get; }
        /** username section of WebFinger without at mark */
        string UserName { get; }
        /** full WebFinger ID without heading @ at mark */
        string WebFinger { get; }
        /** shown on web */
        string DisplayName { get; }
        bool? IsBot { get; }
    }
}