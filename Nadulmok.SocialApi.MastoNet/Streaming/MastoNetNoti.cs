using Mastonet.Entities;
using Nadulmok.SocialApi.Streaming;

namespace Nadulmok.SocialApi.MastoNet.Streaming
{
    public class MastoNetNoti: INoti
    {
        private readonly Notification _notification;

        public MastoNetNoti(Notification notification)
        {
            _notification = notification;
        }

        public NotiType Type =>
            _notification.Type switch
            {
                "follow" => NotiType.Follow,
                "mention" => NotiType.Mention,
                // ReSharper disable once StringLiteralTypo
                "reblog" => NotiType.Renote,
                "favourite" => NotiType.Favourite,
                "poll" => NotiType.Poll,
                _ => NotiType.Unknown
            };

        public IAccount Account => _notification.Account.ToCommon();
        public INote? Note => _notification.Status?.ToCommon();
    }
}