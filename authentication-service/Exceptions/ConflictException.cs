namespace authentication_service.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string? message) : base(message)
        {
        }
    }
}
