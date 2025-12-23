namespace Common.Application.Shared.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class RetryPolicyAttribute : Attribute
{
    private int _retryCount = 3;
    private int _sleepDuration = 1000;

    public int RetryCount
    {
        get => _retryCount;
        set
        {
            _retryCount = value > 1 ? value : 1;
        }
    }

    public int SleepDuration
    {
        get => _sleepDuration;
        set
        {
            _sleepDuration = value > 1 ? value : 1;
        }
    }
}