using Mastonet.Entities;

namespace ChoiceBot.SocialApi.MastoNetAdapter
{
    public static class MastoNetMentionExtension
    {
        public static MastoNetMention ToCommon(this Mention mention)
            => new MastoNetMention(mention);

        internal static Mention ToMastoNet(this IMention mention)
            => MastoNetMention.ToMastoNet(mention);
    }
}