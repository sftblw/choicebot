using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mastonet;
using Nadulmok.SocialApi;
using Visibility = Mastonet.Visibility;

namespace ChoiceBot.BotCommon
{
    public abstract class BotBase : IBotService
    {
        protected IApiClient _client;
        protected IAccount? BotUserInfo { get; private set; }
        
        protected readonly BotPrivacyOption _botPrivacyOption = new BotPrivacyOption
        {
            PreserveContentWarning = false, // TODO
            VisibilityLimit = BotVisibilityLimit.LimitPublicLevel,
            TargetVisibility = StepVisibility.Unlisted
        };

        protected BotBase(IApiClient client)
        {
            _client = client;
        }

        // IBotService
        
        public virtual async Task Initialize()
        {
            BotUserInfo = await _client.GetCurrentUser();
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
                PipeFilterMentionByBot,
                PipeRemoveHtml,
                PipeRemoveMention
            };
        }
        
        // TODO: extract concept
        #region Common bot Services

        private string _ToReplyText(INote status, string replyText)
        {
            IEnumerable<string> mentions = from mention in status.Mentions
                               where !(mention.WebFinger == BotUserInfo.WebFinger
                                       || mention.WebFinger == status.Account.WebFinger)
                               select $"@{mention.WebFinger}";

            string mentionsText = $"@{status.Account.WebFinger} {string.Join(' ', mentions)}".Trim();

            replyText = WebUtility.HtmlDecode(replyText);
            string replyContent = $"{mentionsText} {replyText}";

            return replyContent;
        }

        public async Task ReplyTo(INote note, string replyText, bool trimLength = true)
        {
            const int MaxLetters = 500;
            const string LengthTrimmedDisplayer = "...";
            
            string replyStatus = _ToReplyText(note, replyText);
            
            if (replyStatus.Length > MaxLetters)
            {
                replyStatus = replyStatus.Substring(0, MaxLetters - LengthTrimmedDisplayer.Length)
                              +
                              LengthTrimmedDisplayer;
            }
            
            await _client.CreateNote(replyStatus, _botPrivacyOption.ToBotVisibility(note.Visibility), note.Id);
        }

        protected async Task PipeFilterOnlyMentionToBot(INote note, Func<Task> next)
        {
            if (note?.Mentions?.Any(
                    mention =>
                        mention.WebFinger == BotUserInfo.WebFinger
                        && note.Account.WebFinger != BotUserInfo.WebFinger)
                == true
            )
            {
                await next();
            };
        }

        protected async Task PipeFilterMentionByBot(INote note, Func<Task> next)
        {
            if (note.Account.IsBot != true)
            {
                await next();
            };
        }

        protected static async Task PipeRemoveHtml(INote note, Func<Task> next)
        {
            // strip out html https://stackoverflow.com/a/286825/4394750
            string statusText = note.Content;
            
            statusText = statusText.Replace("<br />", "\r\n").Replace("<br/>", "\r\n");
            statusText = new Regex("<[^>]*>").Replace(statusText, "");
            note.Content = statusText;

            await next();
        }

        protected static async Task PipeRemoveMention(INote note, Func<Task> next)
        {
            note.Content = new Regex("\\@[^\r\n ]+")
                .Replace(note.Content, "").Trim();
            
            await next();
        }

#endregion
    }
}