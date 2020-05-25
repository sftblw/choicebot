namespace Nadulmok.SocialApi
{
    public interface IMention
    {
        string Href { get; }
        string WebFinger { get; }
        string UserName { get; }
    }
}