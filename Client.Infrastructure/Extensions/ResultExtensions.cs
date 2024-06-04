using IGift.Shared.Wrapper;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Client.Infrastructure.Extensions
{
    internal static class ResultExtensions
    {
        internal static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
        {
            //TODO hacer una prueba de que si el endpoint devuelve un Error de servidor o del cualquier tipo, cómo funciona este método. Trabajaría bien? O rompería? Fijarse si debemos poner if response.IsSuccessStatusCode ( en caso de usarlo siempre debería de estar dentro de este método y no en algun servicio ). Fijarse si el middleware captura si hay un error 404. Fijarse si deberíamos hacer siempre throw exception en el servidor para que lo capture el middleware y así devoler siempre un result ( en 
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }

        internal static async Task<IResult> ToResult(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Result>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }

        internal static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return responseObject;
        }
    }
}
