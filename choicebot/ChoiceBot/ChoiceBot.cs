using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mastonet;
using Mastonet.Entities;

namespace choicebot.ChoiceBot
{
    internal class ChoiceBot
    {
        private readonly Random _rand = new Random();
        private readonly MastodonClient _mastoClient;
        private Account _botUserInfo;

        private readonly BotPrivacyOption _botPrivacyOption = new BotPrivacyOption
        {
            PreserveContentWarning = false, // TODO
            VisibilityLimit = BotVisibilityLimit.LimitPublicLevel,
            TargetVisibility = Visibility.Unlisted
        };

        private const string HelpText = "선택할 대상이 없습니다. 선택할 대상을 공백이나 vs(우선)로 구분해서 보내주세요. 골뱅이로 시작하는 내용은 무시됩니다.\r\n\r\nd20 처럼 보내시면 주사위로 인식합니다.";

        public ChoiceBot(MastodonClient client)
        {
            _mastoClient = client;
        }

        public async Task Start()
        {
            _botUserInfo = await _mastoClient.GetCurrentUser();

            TimelineStreaming stream = _mastoClient.GetUserStreaming();            

            // 안됨
            //stream.OnUpdate += async (object sender, StreamUpdateEventArgs e) =>
            //{
            //    var status = e.Status;
            //    await ProcessStatus(status);
            //};

            stream.OnNotification += async (sender, e) =>
            {
                Status status = e.Notification.Status;
                await ProcessStatus(status);
            };

            await stream.Start();
        }

        private async Task ProcessStatus(Status status)
        {
            if (status?.Mentions?.Any(
                mention =>
                    mention.AccountName == _botUserInfo.AccountName
                    && status.Account.AccountName != _botUserInfo.AccountName)
                    == true
            )
            {
                string statusText = _ParseStatusText(status);

                if (string.IsNullOrWhiteSpace(statusText))
                {
                    await ResponseHelp(status);
                    return;
                }

                string[] selectable = _ParseToSelectableItems(statusText);

                // TODO: refactor to middleware-like structure
                if (!selectable.Any())
                {
                    await ResponseHelp(status);
                }
                // for now, support only one dice
                else if (selectable.Length == 1 && Regex.IsMatch(selectable[0].Trim(), "[dD][0-9]+"))
                {
                    await ResponseDice(status, selectable[0].Trim());
                }
                else
                {
                    await _ResponseChoice(status, selectable);
                }
            }
        }

        private async Task ResponseDice(Status status, string diceExpression)
        {
            const string diceErrorMsg = "주사위 숫자를 확인할 수 없습니다. 숫자가 1보다 큰지 확인해보세요.";

            string diceNumStr = Regex.Match(diceExpression.Trim(), "[dD]([0-9]+)").Groups[1].Value;
            int diceNum;

            try { 
                diceNum = int.Parse(diceNumStr);
                if (diceNum <= 1) { throw new ArgumentException("diceNum is less than 1"); }
            }
            catch (Exception ex)
            {
                await _ReplyWithText(status, diceErrorMsg + "\r\n\r\n" + $"에러 메시지: {ex.Message}");
                return;
            }

            string diceReplyStr = $"{_rand.Next(1, diceNum)} ({diceNum}면체 주사위)";

            await _ReplyWithText(status, diceReplyStr);
        }

        private async Task _ResponseChoice(Status status, IReadOnlyList<string> selectable)
        {
            string selection = selectable[_rand.Next(selectable.Count())].Trim();
            await _ReplyWithText(status, selection);
        }

        private async Task ResponseHelp(Status status)
        {
            await _ReplyWithText(status, HelpText);
        }

        private static string[] _ParseToSelectableItems(string statusText)
        {
            string[] selectable;

            const string vsSepRegexStr = "((^|[ \r\n]+)([Vv][Ss]\\.?)(($|[ \r\n]+)([Vv][Ss]\\.?))*($|[ \r\n]+))";

            if (Regex.IsMatch(statusText, vsSepRegexStr, RegexOptions.ExplicitCapture | RegexOptions.Compiled))
            {
                selectable = Regex.Split(statusText, vsSepRegexStr, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
                selectable = selectable.Where(item => !string.IsNullOrWhiteSpace(item)).Select(item => item.Trim()).ToArray();
            }
            else
            {
                selectable = statusText.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }

            return selectable;
        }

        private async Task _ReplyWithText(Status status, string replyText)
        {
            IEnumerable<string> mentions = from mention in status.Mentions
                               where !(mention.AccountName == _botUserInfo.AccountName || mention.AccountName == status.Account.AccountName)
                               select $"@{mention.AccountName}";

            string mentionsText = $"@{status.Account.AccountName} {string.Join(' ', mentions)}".Trim();

            replyText = WebUtility.HtmlDecode(replyText);
            string replyContent = $"{mentionsText} {replyText}";

            await _mastoClient.PostStatus(replyContent, _botPrivacyOption.ToBotVisibility(status.Visibility), status.Id);
        }

        private static string _ParseStatusText(Status status)
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
