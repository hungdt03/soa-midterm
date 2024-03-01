using System.Runtime.Serialization;

namespace tution_service.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string? message) : base(message)
        {
        }

    }
}