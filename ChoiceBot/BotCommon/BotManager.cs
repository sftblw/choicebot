using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mastonet;
using Mastonet.Entities;
using Nadulmok.SocialApi;
using Nadulmok.SocialApi.Streaming;

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
                    await ProcessStatus(noti.Note);
                }
            };
            await streaming.Start();
        }

        /** Process incoming notes with note.
         * Can be manually called for testing or queued processing (second is not implemented yet)
         */
        public async Task ProcessStatus(INote note)
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