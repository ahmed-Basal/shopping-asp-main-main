namespace api.halper
{
    public class ResponseAPI
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ResponseAPI(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            // Use the null-coalescing operator to provide a default message if none is given
            Message = message ?? GetMessageFromStatusCode(statusCode);
        }

        public ResponseAPI(int statusCode, string message = null, string result = null) : this(statusCode, message)
        {
        }

        private string GetMessageFromStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Done",
                400 => "Bad Request",
                401 => "Un Authorized",
                500 => "server Error",
                _ => null
            };
        }
    }
}
