using Mastonet.Entities;

namespace ChoiceBot.SocialApi.MastoNetAdapter
{
    public class MastoNetAccount: IAccount
    {
        private readonly Account _account;

        public MastoNetAccount(Account account)
        {
            _account = account;
        }
        
        public string Id => _account.Id.ToString();
        public string UserName => _account.UserName;
        public string WebFinger => _account.AccountName;
        public string DisplayName => _account.DisplayName;
        public bool? IsBot => _account.Bot;

        internal static Account ToMastoNet(IAccount account) =>
            new Account()
            {
                Id = long.Parse(account.Id),
                UserName = account.UserName,
                AccountName = account.WebFinger,
                DisplayName = account.DisplayName,
                Bot = account.IsBot,
            };
    }
}