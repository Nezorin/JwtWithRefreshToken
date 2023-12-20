
using System.Net;

namespace WebApi.Middleware
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception)
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            }
        }
    }
}
