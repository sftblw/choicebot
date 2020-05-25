using Mastonet.Entities;

namespace Nadulmok.SocialApi.MastoNet
{
    public static class MastoNetMentionExtension
    {
        public static MastoNetMention ToCommon(this Mention mention)
            => new MastoNetMention(mention);

        internal static Mention ToMastoNet(this IMention mention)
            => MastoNetMention.ToMastoNet(mention);
    }
}