using System.Threading.Tasks;
using ChoiceBot.SocialApi.MastoNetAdapter.Streaming;
using ChoiceBot.SocialApi.Streaming;
using Mastonet;

namespace ChoiceBot.SocialApi.MastoNetAdapter
{
    public class MastoNetClient: IApiClient
    {
        internal readonly Mastonet.MastodonClient _client;

        public MastoNetClient(MastodonClient client)
        {
            _client = client;
        }

        public async Task<IAccount> GetCurrentUser() =>
            new MastoNetAccount(await _client.GetCurrentUser());

        public Task CreateNote(string content, ICommonVisibility visibility, string? replyNoteId)
            => _client.PostStatus(
                status: content,
                visibility: visibility.ToMastoNet(),
                replyStatusId: replyNoteId != null ? (long?)long.Parse(replyNoteId) : null);

        public IUserStreaming GetUserStreaming()
            => _client.GetUserStreaming().ToCommon();
    }

    public static class MastoNetClientExtension
    {
        public static IApiClient ToCommon(this Mastonet.MastodonClient client)
            => new MastoNetClient(client);
    }
}