using System.Threading.Tasks;
using Nadulmok.SocialApi.Streaming;

namespace Nadulmok.SocialApi
{
    public interface IApiClient
    {
        Task<IAccount> GetCurrentUser();

        Task CreateNote(string content, ICommonVisibility visibility, string? replyNoteId);
        IUserStreaming GetUserStreaming();
    }
}