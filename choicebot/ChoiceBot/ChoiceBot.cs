using Mastonet;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private BotPrivacyOption BotPrivacyOption = new BotPrivacyOption()
        {
            PreserveContentWarning = false, // TODO
            VisibilityLimit = BotVisibilityLimit.LimitOpenness,
            TargetVisibility = Visibility.Unlisted
        };

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
                    await ResponseHelp(status);
                    return;
                }

                string[] selectable = ParseToSelectableItems(statusText);

                // TODO: refactor to middleware-like structure
                if (selectable.Count() == 0)
                {
                    await ResponseHelp(status);
                }
                // for now, support only one dice
                else if (selectable.Count() == 1 && Regex.IsMatch(selectable[0].Trim(), "[dD][0-9]+"))
                {
                    await ResponseDice(status, selectable[0].Trim());
                }
                else
                {
                    await ResponseChoice(status, selectable);
                }
            }
        }

        private async Task ResponseDice(Status status, string diceExpression)
        {
            const string diceErrorMsg = "주사위 숫자를 확인할 수 없습니다. 숫자가 1보다 큰지 확인해보세요.";

            string diceNumStr = Regex.Match(diceExpression.Trim(), "[dD]([0-9]+)").Groups[1].Value;
            int diceNum = 0;

            try { 
                diceNum = int.Parse(diceNumStr);
                if (diceNum <= 1) { throw new ArgumentException("diceNum is less than 1"); }
            }
            catch
            {
                await _ReplyWithText(status, diceErrorMsg);
            }

            string diceReplyStr = $"{rand.Next(1, diceNum)} ({diceNum}면체 주사위)";

            await _ReplyWithText(status, diceReplyStr);
        }

        private async Task ResponseChoice(Status status, string[] selectable)
        {
            var selection = selectable[rand.Next(selectable.Count())].Trim();
            await _ReplyWithText(status, selection);
        }

        private async Task ResponseHelp(Status status)
        {
            await _ReplyWithText(status, helpText);
        }

        private static string[] ParseToSelectableItems(string statusText)
        {
            string[] selectable = null;

            const string vsSepRegexStr = "((^|[ \r\n]+)([Vv][Ss]\\.?)(($|[ \r\n]+)([Vv][Ss]\\.?))*($|[ \r\n]+))";

            if (Regex.IsMatch(statusText, vsSepRegexStr, RegexOptions.ExplicitCapture | RegexOptions.Compiled))
            {
                selectable = Regex.Split(statusText, vsSepRegexStr, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
                selectable = selectable.Where(item => !string.IsNullOrWhiteSpace(item)).Select(item => item.Trim()).ToArray();
            }
            else
            {
                selectable = statusText?.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }

            return selectable;
        }

        private async Task _ReplyWithText(Status status, string replyText)
        {
            var mentions = from mention in status.Mentions
                               where !(mention.AccountName == botUserInfo.AccountName || mention.AccountName == status.Account.AccountName)
                               select $"@{mention.AccountName}";

            var mentionsText = $"@{status.Account.AccountName} {string.Join(' ', mentions)}".Trim();

            replyText = WebUtility.HtmlDecode(replyText);
            var replyContent = $"{mentionsText} {replyText}";

            await mastoClient.PostStatus(replyContent, BotPrivacyOption.ToBotVisibility(status.Visibility), status.Id);
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
