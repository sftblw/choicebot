using System;
using System.Collections.Generic;

namespace Nadulmok.SocialApi.Dummy
{
    public class DummyNote: INote
    {
        public static long LastNodeId = 0;
        public string Id { get; set; } = (LastNodeId++).ToString();
        public string UriPrefix { get; set; } = "https://example.com/notes/";
        public string Uri => UriPrefix + Id;
        public IAccount Account { get; set; } = new DummyAccount();

        public readonly List<IMention> DummyMentions = new List<IMention>();
        public IEnumerable<IMention> Mentions => DummyMentions;

        public ICommonVisibility Visibility { get; set; }
            = new DummyVisibility {Visibility = StepVisibility.Public, LocalOnly = false};
        
        public string Content { get; set; } = "";
    }
}