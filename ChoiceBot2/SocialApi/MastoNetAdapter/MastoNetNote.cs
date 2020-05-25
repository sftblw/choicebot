using System.Collections.Generic;
using System.Linq;
using Mastonet.Entities;

namespace ChoiceBot.SocialApi.MastoNetAdapter
{
    public class MastoNetNote: INote
    {
        private Status _status;

        public MastoNetNote(Status status)
        {
            _status = status;
        }

        public string Id => _status.Id.ToString();
        public string Uri => _status.Uri;
        public IAccount Account => _status.Account.ToCommon();
        public IEnumerable<IMention> Mentions => _status.Mentions.Select(x => x.ToCommon());

        public ICommonVisibility Visibility
        {
            get => _status.Visibility.ToCommon(); 
            set => _status.Visibility = value.Visibility.ToMastoNetStepVisibility();
        }

        public string Content
        {
            get => _status.Content;
            set => _status.Content = value;
        }

        internal static Status ToMastoNet(INote note) =>
            new Status
            {
                Id = long.Parse(note.Id),
                Uri = note.Uri,
                Account = note.Account.ToMastoNet(),
                Mentions = note.Mentions.Select(x => x.ToMastoNet())
            };
    }
}