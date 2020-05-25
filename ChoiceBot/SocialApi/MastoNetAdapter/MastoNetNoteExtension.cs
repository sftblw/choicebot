using Mastonet.Entities;

namespace ChoiceBot.SocialApi.MastoNetAdapter
{
    public static class MastoNetNoteExtension
    {
        internal static INote ToCommon(this Status status)
            => new MastoNetNote(status);
        
        internal static Status ToMastoNet(this INote note)
            => MastoNetNote.ToMastoNet(note);
    }
}