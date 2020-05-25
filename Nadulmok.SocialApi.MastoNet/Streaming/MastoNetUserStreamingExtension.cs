using Mastonet;
using Nadulmok.SocialApi.Streaming;

namespace Nadulmok.SocialApi.MastoNet.Streaming
{
    public static class MastoNetUserStreamingExtension
    {
        public static IUserStreaming ToCommon(this TimelineStreaming streaming)
            => new MastoNetUserStreaming(streaming);
    }
}