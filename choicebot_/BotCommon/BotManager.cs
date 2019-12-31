using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mastonet;
using Mastonet.Entities;

namespace choicebot_.BotCommon
{
    public class BotManager
    {
        private readonly MastodonClient _mastoClient;
        private List<StatusProcessor> _processors = new List<StatusProcessor>();
        
        public BotManager(MastodonClient client)
        {
            _mastoClient = client;
        }
        
        public void AddBot<TBot>() where TBot: IBotService, new()
        {
            var bot = new TBot();
            bot.MastoClient = this._mastoClient;
            bot.Initialize();
            _processors.AddRange(bot.BuildPipeline());
        }
        
        public async Task Start()
        {
            TimelineStreaming stream = _mastoClient.GetUserStreaming();

            stream.OnNotification += async (sender, e) =>
            {
                Status status = e.Notification.Status;
                await _processStatus(status);
            };

            await stream.Start();
        }

        private async Task _processStatus(Status status)
        {
            foreach (var process in _processors)
            {
                bool next = false;
                async Task SetNext()
                {
                    next = true;
                    await Task.CompletedTask;
                }

                await process(status, SetNext);
                if (!next)
                {
                    return;
                }
            }
        }
    }
}