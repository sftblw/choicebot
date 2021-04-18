using System.Collections.Generic;

namespace Nadulmok.SocialApi
{
    public interface INote
    {
        /// <summary>
        /// on MastoNet, It's long number.
        /// </summary>
        string Id { get; }
        string Uri { get; }
        IAccount Account { get; }
        IEnumerable<IMention>? Mentions { get; }
        /// <summary>HTML Content</summary>
        ICommonVisibility Visibility { get; set; }
        string Content { get; set; }
    }
}