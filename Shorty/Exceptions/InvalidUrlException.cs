namespace Shorty.Exceptions
{
    public class InvalidUrlException(string? message = "Incorrectly formatted URL") : ShortyException(message)
    {
    }
}
