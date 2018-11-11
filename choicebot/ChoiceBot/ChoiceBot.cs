using Mastonet;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace choicebot
{
    internal class ChoiceBot
    {
        private Random rand;
        private MastodonClient mastoClient;
        private Account botUserInfo = null;

        public string helpText = "선택할 대상이 없습니다. 선택할 대상을 공백이나 vs(우선)로 구분해서 보내주세요. 골뱅이로 시작하는 내용은 무시됩니다.";

        public ChoiceBot(MastodonClient client)
        {
            this.rand = new Random();
            this.mastoClient = client;
        }

        public async Task Start()
        {
            botUserInfo = await mastoClient.GetCurrentUser();

            var stream = mastoClient.GetUserStreaming();            

            // 안됨
            //stream.OnUpdate += async (object sender, StreamUpdateEventArgs e) =>
            //{
            //    var status = e.Status;
            //    await ProcessStatus(status);
            //};

            stream.OnNotification += async (object sender, StreamNotificationEventArgs e) =>
            {
                var status = e.Notification.Status;
                await ProcessStatus(status);
            };

            await stream.Start();
        }

        private async Task ProcessStatus(Status status)
        {
            if (status?.Mentions?.Any(
                (mention) =>
                    mention.AccountName == botUserInfo.AccountName
                    && status.Account.AccountName != botUserInfo.AccountName)
                    == true
            )
            {
                string statusText = ParseStatusText(status);

                if (string.IsNullOrWhiteSpace(statusText))
                {
                    await ReplyWithText(status, helpText);
                    return;
                }

                string[] selectable = null;

                string[] vsSeparator = new string[] { " vs. ", " vs ", " Vs. ", " Vs ", " VS. ", " VS " };
                if (vsSeparator.Any((sep) => statusText.Contains(sep)))
                {
                    selectable = statusText.Split(vsSeparator, 999, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    selectable = statusText.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }

                var selection = selectable[rand.Next() % selectable.Count()].Trim();
                await ReplyWithText(status, selection);
            }
        }

        private async Task ReplyWithText(Status status, string replyText)
        {
            var mentions = from mention in status.Mentions
                               where !(mention.AccountName == botUserInfo.AccountName || mention.AccountName == status.Account.AccountName)
                               select $"@{mention.AccountName}";

            var mentionsText = $"@{status.Account.AccountName} {string.Join(' ', mentions)}";

            var replyContent = $"{mentionsText} {replyText}";

            await mastoClient.PostStatus(replyContent, Visibility.Unlisted, status.Id);
        }

        private static string ParseStatusText(Status status)
        {
            // strip out html https://stackoverflow.com/a/286825/4394750
            string statusText = status.Content;
            statusText = statusText.Replace("<br />", "\r\n").Replace("<br/>", "\r\n");
            statusText = new Regex("<[^>]*>").Replace(statusText, "");

            statusText = new Regex("\\@[^\r\n ]+").Replace(statusText, "").Trim();
            return statusText;
        }
    }
}
