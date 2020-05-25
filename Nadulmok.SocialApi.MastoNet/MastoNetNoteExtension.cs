using Mastonet.Entities;

namespace Nadulmok.SocialApi.MastoNet
{
    public static class MastoNetNoteExtension
    {
        internal static INote ToCommon(this Status status)
            => new MastoNetNote(status);
        
        internal static Status ToMastoNet(this INote note)
            => MastoNetNote.ToMastoNet(note);
    }
}