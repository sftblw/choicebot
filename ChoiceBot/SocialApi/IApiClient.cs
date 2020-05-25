using System.Threading.Tasks;
using ChoiceBot.SocialApi.Streaming;

namespace ChoiceBot.SocialApi
{
    public interface IApiClient
    {
        Task<IAccount> GetCurrentUser();

        Task CreateNote(string content, ICommonVisibility visibility, string? replyNoteId);
        IUserStreaming GetUserStreaming();
    }
}