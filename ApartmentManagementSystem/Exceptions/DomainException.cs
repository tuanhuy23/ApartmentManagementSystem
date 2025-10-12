using System.Net;

namespace ApartmentManagementSystem.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
            Code = "";
        }

        public DomainException(string code, string message) : base(message)
        {
            Code = code;
        }
        public DomainException(string code, string message, HttpStatusCode statusCode) : base(message)
        {
            Code = code;
            StatusCode = statusCode;
        }

        public string Code { get; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
