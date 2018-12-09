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

        private const string HelpText = 
              "- 선택: 공백이나 vs로 구분해서 보내주세요\r\n"
            + "- 주사위: (주사위 개수)d(주사위 숫자) 를 보내주세요 (예: d5 2d10 등)\r\n"
            + "- 예아니오: 끝에 예아니오를 적어서 포함해서 보내주세요\r\n";

        public override IEnumerable<StatusProcessor> BuildPipeline()
        {
            var list = new List<StatusProcessor>()
            {
                PipeHelp,
                PipeYesNo,
                PipeDice,
                PipeChoice,
                PipeNotHandledHelp
            };
            
            return base.BuildPipeline().Concat(list);
        }

        private async Task PipeHelp(Status status, Func<Task> next)
        {
            if (!status.Content.Contains("도움말"))
            {
                await next();
                return;
            }

            await ReplyTo(status, HelpText);
        }

        private async Task PipeYesNo(Status status, Func<Task> next)
        {
            if (!status.Content.Contains("예아니오"))
            {
                await next();
                return;
            }

            double randNum = _rand.NextDouble();
            string replyText = $"{((randNum >= 0.5) ? "예" : "아니오")} ({Math.Round(randNum * 100)}%)";
            await ReplyTo(status, replyText);
        }
        
        private async Task PipeDice(Status status, Func<Task> next)
        {
            string diceExpression = status.Content.Trim();
            string[] diceExprList = diceExpression.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var matchQuery = diceExprList.Select(expr => Regex.Match(expr, "([0-9]*?)[dD]([0-9]+)"));
            var matchList = matchQuery as Match[] ?? matchQuery.ToArray();
            
            if (matchList.Any(match => !match.Success) || matchList.Length == 0)
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
            
            if (selectable.Length == 1)
            {
                selection += "\r\n(선택할 항목이 하나밖에 없는 것 같습니다.)";
            }
            
            await ReplyTo(status, selection);
        }
        
        private async Task PipeNotHandledHelp(Status status, Func<Task> next)
        {
            await ReplyTo(status, "선택할 게 없는 것 같습니다. 이렇게 해보세요:\r\n\r\n" + HelpText);
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
