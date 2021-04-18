namespace Nadulmok.SocialApi.Dummy
{
    public class DummyMention: IMention
    {
        public DummyMention(string webFinger, string userName)
        {
            WebFinger = webFinger;
            UserName = userName;
        }

        public DummyMention(DummyAccount from)
        {
            WebFinger = from.WebFinger;
            UserName = from.UserName;
        }

        /** will not implement **/
        public string Href => "https://example.com/users/" + UserName;
        public string WebFinger { get; set; }
        public string UserName { get; set;  }
    }
}