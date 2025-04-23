
namespace BookStore.Services
{
    [Serializable]
    internal class EmailAlreadyException : Exception
    {
        public EmailAlreadyException()
        {
        }

        public EmailAlreadyException(string? message) : base(message)
        {
        }

        public EmailAlreadyException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}