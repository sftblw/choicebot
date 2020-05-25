using System;
using System.Threading.Tasks;
using Mastonet;
using Nadulmok.SocialApi.Streaming;

namespace Nadulmok.SocialApi.MastoNet.Streaming
{
    public class MastoNetUserStreaming: IUserStreaming
    {
        private readonly TimelineStreaming _streaming;

        public MastoNetUserStreaming(TimelineStreaming streaming)
        {
            _streaming = streaming;
        }
        
        public event Action<INoti>? OnNoti;

        public Task Start()
        {
            _streaming.OnNotification += (sender, args) =>
            {
                OnNoti?.Invoke(args.Notification.ToCommon());
            };
            return _streaming.Start();
        }
    }
}