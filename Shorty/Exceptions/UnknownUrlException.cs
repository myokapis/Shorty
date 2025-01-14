namespace Shorty.Exceptions
{
    public class UnknownUrlException : ShortyException
    {
        public UnknownUrlException(string? message = "Unknown Url") : base(message)
        {
        }
    }
}
