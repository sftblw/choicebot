using ChoiceBot.SocialApi.Streaming;
using Mastonet.Entities;

namespace ChoiceBot.SocialApi.MastoNetAdapter.Streaming
{
    public static class MastoNetNotiExtension
    {
        public static INoti ToCommon(this Notification noti)
            => new MastoNetNoti(noti);
    }
}