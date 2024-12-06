using System.Net;
using System.Text.Json;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Http;

namespace IGIFT.Server.Shared.Middleware
{
    public class MyMiddleware
    {
        private readonly RequestDelegate _next;

        public MyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = await Result<string>.FailAsync(error.Message);

                switch (error)
                {

                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                //TODO JsonSerializer esta usando actualmente system.text.json. Deberiamos de tener la interfaz en el constructor y segun el tipo de microservicio que use una o la otra. Podriamos resolverlo teniendo dos middlewares uno con una interfaz y otro con otra.
                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
