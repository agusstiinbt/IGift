using System.Net.Http.Json;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Categoria.Query;
using IGift.Application.Responses.Categoria;
using IGift.Shared.Constants.Controllers;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Categoria
{
    public class CategoriaService : ICategoriaService
    {
        private readonly HttpClient _httpClient;

        public CategoriaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<IEnumerable<CategoriaResponse>>> GetAll()
        {
            var query = new GetAllCategoriaQuery();

            var url = ConstCategoriaController.GetAll;

            var response = await _httpClient.PostAsJsonAsync(url, query);

            return await response.ToResult<IEnumerable<CategoriaResponse>>();
        }
    }
}
