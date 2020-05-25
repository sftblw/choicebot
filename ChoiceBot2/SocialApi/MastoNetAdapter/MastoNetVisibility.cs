using System;
using Mastonet;

namespace ChoiceBot.SocialApi.MastoNetAdapter
{
    public class MastoNetVisibility: ICommonVisibility
    {
        private Mastonet.Visibility _visibility;

        public MastoNetVisibility(Mastonet.Visibility visibility)
        {
            _visibility = visibility;
        }
        
        public bool LocalOnly
        {
            get => false;
            set
            {
                if (value)
                {
                    throw new ArgumentException("Can't set LocalOnly on mastodon");
                }
            }
        }
        public StepVisibility Visibility
        {
            get => _visibility.ToCommonStepVisibility();
            set => _visibility = value.ToMastoNetStepVisibility();
        }
    }
}