using Client.Infrastructure.Extensions;
using IGift.Application.Features.Pedidos.Query;
using IGift.Application.Responses.Pedidos;
using IGift.Shared;
using IGift.Shared.Wrapper;
using System.Net.Http.Json;

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
            var response = await _http.PostAsJsonAsync(AppConstants.Controllers.PeticionesController.GetAll, request);
            var result = await response.ToResult<PaginatedResult<PeticionesResponse>>();
            return result;
        }
    }
}
