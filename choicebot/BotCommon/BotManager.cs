using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChoiceBot.SocialApi;
using ChoiceBot.SocialApi.MastoNetAdapter;
using Mastonet;
using Mastonet.Entities;

namespace ChoiceBot.BotCommon
{
    public class BotManager
    {
        private readonly IApiClient _apiClient;
        private List<StatusProcessor> _processors = new List<StatusProcessor>();
        
        public BotManager(IApiClient client)
        {
            _apiClient = client;
        }
        
        public void AddBot(IBotService bot) 
        {
            bot.Initialize();
            _processors.AddRange(bot.BuildPipeline());
        }
        
        public async Task Start()
        {
            // for now. TODO: refactor out UserStream
            // instead of abstracting UserStream, I decided to use it for now.
            if (_apiClient is MastoNetClient mastoClient) {
                TimelineStreaming stream = mastoClient._client.GetUserStreaming();
                stream.OnNotification += async (sender, e) =>
                {
                    Status status = e.Notification.Status;
                    await _processStatus(status.ToCommon());
                };
                
                await stream.Start();
            }
            else
            {
                throw new NotSupportedException("Currently only MastoNet client will be supported");
            }
        }

        private async Task _processStatus(INote note)
        {
            foreach (var process in _processors)
            {
                bool next = false;
                async Task SetNext()
                {
                    next = true;
                    await Task.CompletedTask;
                }

                await process(note, SetNext);
                if (!next)
                {
                    return;
                }
            }
        }
    }
}