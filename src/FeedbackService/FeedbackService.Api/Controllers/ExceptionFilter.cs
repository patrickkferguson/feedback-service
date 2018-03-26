using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FeedbackService.Api.Controllers
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new ObjectResult(context.Exception.ToString())
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }
}
