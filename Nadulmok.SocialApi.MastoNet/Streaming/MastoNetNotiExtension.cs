using Mastonet.Entities;
using Nadulmok.SocialApi.Streaming;

namespace Nadulmok.SocialApi.MastoNet.Streaming
{
    public static class MastoNetNotiExtension
    {
        public static INoti ToCommon(this Notification noti)
            => new MastoNetNoti(noti);
    }
}