namespace aspnet_todolist.Exceptions
{
    public class ApiError
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? Details { get; set; }

        public ApiError() { }

        public ApiError(string message, int statusCode, string? details = null)
        {
            Message = message;
            StatusCode = statusCode;
            Details = details;
        }
    }
}
