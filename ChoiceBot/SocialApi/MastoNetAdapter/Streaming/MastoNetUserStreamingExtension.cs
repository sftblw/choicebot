using ChoiceBot.SocialApi.Streaming;
using Mastonet;

namespace ChoiceBot.SocialApi.MastoNetAdapter.Streaming
{
    public static class MastoNetUserStreamingExtension
    {
        public static IUserStreaming ToCommon(this TimelineStreaming streaming)
            => new MastoNetUserStreaming(streaming);
    }
}