using System;
using Mastonet.Entities;

namespace ChoiceBot.SocialApi.MastoNetAdapter
{
    public class MastoNetMention: IMention
    {
        private readonly Mention _mention;

        public MastoNetMention(Mention mention)
        {
            _mention = mention;
        }

        public string Href => _mention.Url;
        public string WebFinger => _mention.AccountName;
        public string UserName => _mention.UserName;

        internal static Mention ToMastoNet(IMention mention) =>
            new Mention()
            {
                Id = 0, // will not implement
                Url = mention.Href,
                AccountName = mention.WebFinger,
                UserName = mention.UserName
            };
    }

    
}