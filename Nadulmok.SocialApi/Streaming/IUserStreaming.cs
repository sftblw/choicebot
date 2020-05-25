using System;
using System.Threading.Tasks;

namespace Nadulmok.SocialApi.Streaming
{
    public interface IUserStreaming
    {
        event Action<INoti>? OnNoti;
        Task Start();
    }
}