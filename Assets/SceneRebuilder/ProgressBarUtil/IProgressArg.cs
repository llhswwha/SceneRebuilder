public interface IProgressArg
{
    string GetTitle();

    float GetProgress();

    IProgressArg Clone();
}
