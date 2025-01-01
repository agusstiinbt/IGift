using System.Net.Http.Json;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Responses.Pedidos;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Requests
{
    public class PeticionesService : IPeticionesService
    {
        private readonly HttpClient _http;

        public PeticionesService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IResult<PaginatedResult<PeticionesResponse>>> GetAll(GetAllPeticionesQuery request)
        {
            var response = await _http.PostAsJsonAsync(ConstPeticionesController.GetAll, request);
            var result = await response.ToResult<PaginatedResult<PeticionesResponse>>();
            return result;
        }
    }
}
