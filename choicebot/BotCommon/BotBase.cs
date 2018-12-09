using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using choicebot.ChoiceBotNS;
using Mastonet;
using Mastonet.Entities;

namespace choicebot.BotCommon
{
    public abstract class BotBase : IBotService
    {
        public MastodonClient MastoClient { get; set; }
        protected Account BotUserInfo { get; private set; }
        
        protected readonly BotPrivacyOption _botPrivacyOption = new BotPrivacyOption
        {
            PreserveContentWarning = false, // TODO
            VisibilityLimit = BotVisibilityLimit.LimitPublicLevel,
            TargetVisibility = Visibility.Unlisted
        };
        
        // IBotService
        
        public virtual async Task Initialize()
        {
            BotUserInfo = await MastoClient.GetCurrentUser();
        }

        /// <summary>
        /// Creates request processing pipeline. Base pipeline has one which rips out HTML.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<StatusProcessor> BuildPipeline()
        {
            return new List<StatusProcessor>
            {
                PipeFilterOnlyMentionToBot,
                PipeRemoveHtml,
                PipeRemoveMention
            };
        }

#region Common bot Services

        public async Task ReplyWithText(Status status, string replyText)
        {
            IEnumerable<string> mentions = from mention in status.Mentions
                               where !(mention.AccountName == BotUserInfo.AccountName || mention.AccountName == status.Account.AccountName)
                               select $"@{mention.AccountName}";

            string mentionsText = $"@{status.Account.AccountName} {string.Join(' ', mentions)}".Trim();

            replyText = WebUtility.HtmlDecode(replyText);
            string replyContent = $"{mentionsText} {replyText}";

            await MastoClient.PostStatus(replyContent, _botPrivacyOption.ToBotVisibility(status.Visibility), status.Id);
        }

        protected async Task PipeFilterOnlyMentionToBot(Status status, Func<Task> next)
        {
            if (status?.Mentions?.Any(
                    mention =>
                        mention.AccountName == BotUserInfo.AccountName
                        && status.Account.AccountName != BotUserInfo.AccountName)
                == true
            )
            {
                await next();
            };
        }
        
        protected static async Task PipeRemoveHtml(Status status, Func<Task> next)
        {
            // strip out html https://stackoverflow.com/a/286825/4394750
            string statusText = status.Content;
            
            statusText = statusText.Replace("<br />", "\r\n").Replace("<br/>", "\r\n");
            statusText = new Regex("<[^>]*>").Replace(statusText, "");
            status.Content = statusText;

            await next();
        }

        protected static async Task PipeRemoveMention(Status status, Func<Task> next)
        {
            status.Content = new Regex("\\@[^\r\n ]+").Replace(status.Content, "").Trim();
            
            await next();
        }

#endregion
    }
}