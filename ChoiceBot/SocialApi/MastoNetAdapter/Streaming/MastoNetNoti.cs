using System.Diagnostics;
using ChoiceBot.SocialApi.Streaming;
using Mastonet.Entities;

namespace ChoiceBot.SocialApi.MastoNetAdapter.Streaming
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