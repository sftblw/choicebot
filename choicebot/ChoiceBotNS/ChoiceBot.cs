using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using choicebot.BotCommon;
using Mastonet;
using Mastonet.Entities;

namespace choicebot.ChoiceBotNS
{
    public class ChoiceBot: BotBase
    {
        private readonly Random _rand = new Random();

        private const string HelpText = "선택할 대상이 없습니다. 선택할 대상을 공백이나 vs(우선)로 구분해서 보내주세요. 골뱅이로 시작하는 내용은 무시됩니다.\r\n\r\nd20 처럼 보내시면 주사위로 인식합니다.";

        public override IEnumerable<StatusProcessor> BuildPipeline()
        {
            var list = new List<StatusProcessor>()
            {
                PipeYesNo,
                PipeDice,
                PipeChoice,
                PipeHelp
            };
            
            return base.BuildPipeline().Concat(list);
        }
        
        private async Task PipeYesNo(Status status, Func<Task> next)
        {
            // TODO
            await next();
        }
        
        private async Task PipeDice(Status status, Func<Task> next)
        {
            string diceExpression = status.Content.Trim();
            string[] diceExprList = diceExpression.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var matchQuery = diceExprList.Select(expr => Regex.Match(expr, "([0-9]*?)[dD]([0-9]+)"));
            var matchList = matchQuery as Match[] ?? matchQuery.ToArray();
            
            if (matchList.Any(match => !match.Success))
            {
                await next();
                return;
            }
            
            const string diceErrorMsg = "주사위 숫자를 확인할 수 없습니다. 숫자가 1보다 큰지 확인해보세요.";

            try
            {
                string[] diceResults = matchList.Select(match =>
                {
                    string diceCountStr = match.Groups[1].Value;
                    diceCountStr = string.IsNullOrWhiteSpace(diceCountStr) ? "1" : diceCountStr;
                    string diceNumStr = match.Groups[2].Value;

                    int diceCount;
                    int diceNum;

                    diceCount = int.Parse(diceCountStr);
                    diceNum = int.Parse(diceNumStr);
                    if (diceNum <= 1)
                    {
                        throw new ArgumentException("diceNum is less than or equal to 1");
                    }

                    string diceRollStr = string.Join(", ", Enumerable
                        .Repeat(0, diceCount)
                        .Select(_ => _rand.Next(1, diceNum + 1)).ToArray());

                    return $"{diceRollStr} ({diceNum}면체)";
                }).ToArray();

                string diceReplyStr = string.Join("\r\n", diceResults);
                if (diceResults.Length > 1)
                {
                    diceReplyStr = "\r\n" + diceReplyStr;
                }
                
                await ReplyTo(status, diceReplyStr);
            }
            catch (Exception ex)
            {
                await ReplyTo(status, diceErrorMsg + "\r\n\r\n" + $"에러 메시지: {ex.Message}");
                return;
            }
        }
        
        private async Task PipeChoice(Status status, Func<Task> next)
        {
            string[] selectable = _ParseToSelectableItems(status.Content);

            if (!selectable.Any())
            {
                await next();
                return;
            }
            
            string selection = selectable[_rand.Next(selectable.Count())].Trim();
            await ReplyTo(status, selection);
        }
        
        private async Task PipeHelp(Status status, Func<Task> next)
        {
            await ReplyTo(status, HelpText);
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
    }
}
