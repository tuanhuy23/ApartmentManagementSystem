using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.Exceptions
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var error = ExtractErrorResponseFromContext(context);

            context.HttpContext.Response.StatusCode = (int)error.StatusCode;
            context.Result = new JsonResult(error);
        }

        private static ErrorResponse ExtractErrorResponseFromContext(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case DomainException domainValidationEx:
                    return domainValidationEx.CreateErrorResponse();
                    ;
                default:
                    return context.Exception.CreateErrorResponse();
            }
        }
    }
}
