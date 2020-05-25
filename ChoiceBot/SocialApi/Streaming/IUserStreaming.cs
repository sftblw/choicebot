using System;
using System.Threading.Tasks;

namespace ChoiceBot.SocialApi.Streaming
{
    public interface IUserStreaming
    {
        event Action<INoti>? OnNoti;
        Task Start();
    }
}