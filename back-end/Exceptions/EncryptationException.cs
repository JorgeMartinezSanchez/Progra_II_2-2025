namespace back_end.Exceptions
{
    public class EncryptionException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }

        public EncryptionException(string message, string errorCode = "ENCRYPTION_ERROR", int statusCode = 500) 
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        public EncryptionException(string message, Exception innerException, string errorCode = "ENCRYPTION_ERROR", int statusCode = 500) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}
