using System;

namespace ChoiceBot.SocialApi
{
    public interface IMention
    {
        string Href { get; }
        string WebFinger { get; }
        string UserName { get; }
    }
}