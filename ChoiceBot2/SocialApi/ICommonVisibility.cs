namespace ChoiceBot.SocialApi
{
    public interface ICommonVisibility
    {
        bool LocalOnly { get; set; }
        StepVisibility Visibility { get; set; }
    }
}