using System.Net.Http.Json;
using System.Text.Json;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Extensions
{
    public static class ResultExtensions
    {
        public static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<Result<T>>();
                    //var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
                    //{
                    //    PropertyNameCaseInsensitive = true,
                    //    ReferenceHandler = ReferenceHandler.Preserve
                    //});
                    //return responseObject!;
                }
                catch (Exception e)
                {
                    return await Result<T>.FailAsync(e.Message);
                }
            }

            return await Result<T>.FailAsync(HandleMessage(response));
        }

        public static async Task<IResult> ToResult(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<Result>();
                }
                catch (Exception e)
                {

                    return await Result.FailAsync(e.Message);
                }
            }
            return await Result.FailAsync(HandleMessage(response));
        }

        public static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return responseObject;
        }

        private static string HandleMessage(HttpResponseMessage response)
        {
            string message = string.Empty;

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                message = "404; Servicio no encontrado";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                message = "400; Error en la petición";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                message = "401; No autorizado";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                message = "403; No tiene permisos para realizar esta operación";
            }
            else
            {
                message = "Ha ocurrido un error inesperado";
            }

            return message;
        }
    }
}
