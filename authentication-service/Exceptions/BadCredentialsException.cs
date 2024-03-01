namespace authentication_service.Exceptions
{
    public class BadCredentialsException : Exception
    {
        public BadCredentialsException(string? message) : base(message)
        {
        }
    }
}
