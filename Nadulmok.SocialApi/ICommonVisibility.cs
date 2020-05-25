namespace Nadulmok.SocialApi
{
    public interface ICommonVisibility
    {
        bool LocalOnly { get; set; }
        StepVisibility Visibility { get; set; }
    }
}