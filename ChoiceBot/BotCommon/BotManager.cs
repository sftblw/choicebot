using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChoiceBot.SocialApi;
using ChoiceBot.SocialApi.MastoNetAdapter;
using ChoiceBot.SocialApi.Streaming;
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
            IUserStreaming streaming = _apiClient.GetUserStreaming();
            streaming.OnNoti += async (INoti noti) =>
            {
                if (noti.Note != null)
                {
                    await _processStatus(noti.Note);
                }
            };
            await streaming.Start();
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