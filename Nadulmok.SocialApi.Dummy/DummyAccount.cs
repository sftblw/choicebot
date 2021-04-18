namespace Nadulmok.SocialApi.Dummy
{
    public class DummyAccount : IAccount
    {
        public string Id { get; set; } = "12345678–1234–1234–1234–1234567890ab";

        private string _userName = "test_account";
        public string UserName
        {
            get => _userName;
            set
            {
                var fingerSep = WebFinger.Split("@");
                var domain = fingerSep.Length >= 2 ? fingerSep[1] : "example.com";
                _userName = value;
                WebFinger = value + "@" + domain;
            }
        }
        
        public string WebFinger { get; set; } = "test_account@example.com";
        public string DisplayName { get; set; } = "I'm a Test Account";
        public bool? IsBot { get; set; } = false;
    }
}