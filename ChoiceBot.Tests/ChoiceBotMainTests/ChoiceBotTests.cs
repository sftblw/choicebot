using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChoiceBot.BotCommon;
using Nadulmok.SocialApi.Dummy;
using Xunit;

namespace ChoiceBot.Tests.ChoiceBotMainTests
{
    public class ChoiceBotTests
    {
        public class TestBotManager
        {
            public BotManager Manager { get; }
            public DummyApiClient ApiClient { get; }

            public TestBotManager()
            {
                ApiClient = DummyFactory.CreateDummyApiClient();
                var botModule = new ChoiceBotMain.ChoiceBot(ApiClient);
                
                Manager = new BotManager(ApiClient);
                Manager.AddBot(botModule);
            }
            
            public void ClearNotesCreated() { ApiClient.CreatedNotes.Clear(); }

            public void ProcessNoteByOtherToMe(string body, string userName = "test_user")
            {
                var dummyNote = new DummyNote {Content = body};
                
                dummyNote.DummyMentions.Add(new DummyMention(ApiClient.CurrentUser));
                ((DummyAccount) dummyNote.Account).UserName = userName;
                
                Manager.ProcessStatus(dummyNote).Wait();
            }
        }
        
        public TestBotManager CreateDummyBotManager()
        {
            return new TestBotManager();
        }
        
        private readonly Regex idFilterRegex = new Regex("(@[\\w_.]+)+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        [Theory]
        [InlineData("@choicebot asdf vs gg", "asdf", "gg")]
        [InlineData("@choicebot asdf\ngg", "asdf", "gg")]
        [InlineData("@choicebot@example.com asdf\ngg", "asdf", "gg")]
        [InlineData("asdf<br/>gg", "asdf", "gg")]
        [InlineData("<p>asdf</p><p>gg</p>", "asdf", "gg")]
        [InlineData("asdf @choicebot@example.com gg", "asdf", "gg")]
        [InlineData("<p>asdf</p><p>vs</p><p>gg</p>", "asdf", "gg")]
        [InlineData("@choicebot my\nawesome\nchoice\n\n\nvs\n\n\nmy\nanother\nchoice", "my\nawesome\nchoice", "my\nanother\nchoice")]
        [InlineData("\n\n\n\n\nmy\nawesome\nchoice\n\n\nvs\n\n\nmy\nanother\nchoice", "my\nawesome\nchoice", "my\nanother\nchoice")]
        public void Should_be_separated_by_vs(string body, params string[] results)
        {
            HashSet<string> possibleResults = results.ToHashSet();
            HashSet<string> revealedResults = new HashSet<string>();
            
            var manager = CreateDummyBotManager();

            const int maxRepeat = 20;
            foreach (var _ in Enumerable.Range(0, maxRepeat))
            {
                manager.ProcessNoteByOtherToMe(body);
                Assert.Single(manager.ApiClient.CreatedNotes);
                
                DummyNote note = manager.ApiClient.CreatedNotes.Dequeue();
                string resultText = idFilterRegex.Replace(note.Content, "").Trim();
                
                Assert.Contains(resultText, possibleResults);
                revealedResults.Add(resultText);

                if (possibleResults.Count == revealedResults.Count)
                {
                    break;
                }
            }
        }
    }
}