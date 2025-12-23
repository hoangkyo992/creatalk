namespace Common.Application.Shared.Exceptions;

public class RetryableException : Exception
{
    public RetryableException()
    {
    }

    public RetryableException(string message)
        : base(message)
    {
    }

    public RetryableException(string message, Exception inner)
        : base(message, inner)
    {
    }
}