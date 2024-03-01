namespace ibanking_server.Exceptions
{
    public class InvalidOTPException : Exception
    {
        public InvalidOTPException(string? message) : base(message)
        {
        }
    }
}
