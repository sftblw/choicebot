using Mastonet.Entities;

namespace ChoiceBot.SocialApi.MastoNetAdapter
{
    public static class MastoNetAccountExtension
    {
        public static IAccount ToCommon(this Account account)
            => new MastoNetAccount(account);

        internal static Account ToMastoNet(this IAccount account)
            => MastoNetAccount.ToMastoNet(account);
    }
}