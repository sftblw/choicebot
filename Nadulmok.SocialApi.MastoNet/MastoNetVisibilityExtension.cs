using System;

namespace Nadulmok.SocialApi.MastoNet
{
    public static class MastoNetVisibilityExtension
    {
        public static MastoNetVisibility ToCommon(this Mastonet.Visibility visibility)
            => new MastoNetVisibility(visibility);

        internal static Mastonet.Visibility ToMastoNet(this ICommonVisibility visibility)
            => visibility.Visibility.ToMastoNetStepVisibility();
        
        public static StepVisibility ToCommonStepVisibility(this Mastonet.Visibility visibility)
        {
            return visibility switch
            {
                Mastonet.Visibility.Public => StepVisibility.Public,
                Mastonet.Visibility.Unlisted => StepVisibility.Unlisted,
                Mastonet.Visibility.Private => StepVisibility.Private,
                Mastonet.Visibility.Direct => StepVisibility.Direct,
                _ => throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null)
            };
        }

        internal  static Mastonet.Visibility ToMastoNetStepVisibility(this StepVisibility visibility)
        {
            return visibility switch
            {
                StepVisibility.Public => Mastonet.Visibility.Public,
                StepVisibility.Unlisted => Mastonet.Visibility.Unlisted,
                StepVisibility.Private => Mastonet.Visibility.Private,
                StepVisibility.Direct => Mastonet.Visibility.Direct,
                _ => throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null)
            };
        }
    }
}