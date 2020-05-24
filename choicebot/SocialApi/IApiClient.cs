using System.Threading.Tasks;

namespace ChoiceBot.SocialApi
{
    public interface IApiClient
    {
        Task<IAccount> GetCurrentUser();

        Task CreateNote(string content, ICommonVisibility visibility, string? replyNoteId);
    }
}