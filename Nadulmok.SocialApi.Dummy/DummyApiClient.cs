using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Nadulmok.SocialApi.Streaming;

namespace Nadulmok.SocialApi.Dummy
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class DummyApiClient: IApiClient
    {
        public DummyAccount CurrentUser { get; set; } = new DummyAccount();

        public Task<IAccount> GetCurrentUser() => Task.FromResult(CurrentUser as IAccount);

        
        public readonly Queue<DummyNote> CreatedNotes = new Queue<DummyNote>();
        public Task CreateNote(string content, ICommonVisibility visibility, string? replyNoteId)
        {
            var note = new DummyNote
            {
                Content = content,
                Visibility = visibility,
                Account = CurrentUser
            };

            CreatedNotes.Enqueue(note);
            
            return Task.CompletedTask;
        }

        public IUserStreaming GetUserStreaming()
        {
            throw new NotImplementedException();
        }
    }
}