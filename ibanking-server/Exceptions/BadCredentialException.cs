namespace ibanking_server.Exceptions
{
    public class BadCredentialException : Exception
    {
        public BadCredentialException(string? message) : base(message)
        {
        }
    }
}
