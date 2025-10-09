using System.Net;

namespace ApartmentManagementSystem.Exceptions
{
    public class ErrorResponse
    {
        public ErrorResponse(string errorCode, string message, int statusCode)
        {
            ErrorCode = errorCode;
            Message = message;
            StatusCode = statusCode;
        }

        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }

    public static class ExceptionExtensions
    {
        public static ErrorResponse CreateErrorResponse(this DomainException ex)
        {
            return new ErrorResponse(ex.Code, ex.Message, ex.StatusCode);
        }

        public static ErrorResponse CreateErrorResponse(this Exception ex) =>
            new ErrorResponse("500", ex.Message, (int)HttpStatusCode.InternalServerError);
    }
}
