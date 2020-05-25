namespace Nadulmok.SocialApi
{
    public interface IAccount
    {
        string Id { get; }
        string UserName { get; }
        string WebFinger { get; }
        string DisplayName { get; }
        bool? IsBot { get; }
    }
}