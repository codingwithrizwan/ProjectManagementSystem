namespace PMS.Web.Exceptions
{
    public class ApiClientException : Exception
    {
        public int StatusCode { get; }

        public ApiClientException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
