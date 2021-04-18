namespace Nadulmok.SocialApi.Dummy
{
    public class DummyVisibility: ICommonVisibility
    {
        public bool LocalOnly { get; set; }
        public StepVisibility Visibility { get; set; }
    }
}