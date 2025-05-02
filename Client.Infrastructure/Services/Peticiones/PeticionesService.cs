using System.Net.Http.Json;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Responses.Peticiones;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Peticiones
{
    public class PeticionesService : IPeticionesService
    {
        private readonly HttpClient _http;

        public PeticionesService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PaginatedResult<PeticionesResponse>> GetAll(GetAllPeticionesQuery request)
        {
            var response = await _http.PostAsJsonAsync(AppConstants.Controller.Peticiones.GetAll, request);
            var result = await response.ToPaginatedResult<PeticionesResponse>();
            return result;
        }
    }
}
