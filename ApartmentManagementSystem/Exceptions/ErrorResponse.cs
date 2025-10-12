using System.Net;

namespace ApartmentManagementSystem.Exceptions
{
    public class ErrorResponse
    {
        public ErrorResponse(string errorCode, string message, HttpStatusCode statusCode)
        {
            ErrorCode = errorCode;
            Message = message;
            StatusCode = statusCode;
        }

        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public static class ExceptionExtensions
    {
        public static ErrorResponse CreateErrorResponse(this DomainException ex)
        {
            return new ErrorResponse(ex.Code, ex.Message, ex.StatusCode);
        }

        public static ErrorResponse CreateErrorResponse(this Exception ex) =>
            new ErrorResponse("INTERNAL_SERVER_ERROR", ex.Message, HttpStatusCode.InternalServerError);
    }
}
