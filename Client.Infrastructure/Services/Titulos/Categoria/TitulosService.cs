using System.Net.Http.Json;
using Client.Infrastructure.Extensions;
using IGift.Application.Responses.Titulos;
using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Conectado;
using IGift.Application.Responses.Titulos.Desconectado;
using IGift.Shared.Constants.Controllers;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Titulos.Categoria
{
    public class TitulosService : ITitulosService
    {
        private readonly HttpClient _httpClient;

        public TitulosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<IEnumerable<CategoriaResponse>>> GetAllCategorias()
        {
            var query = new GetAllCategoriaQuery();

            var url = TitulosController.GetAllCategorias;

            var response = await _httpClient.PostAsJsonAsync(url, query);

            return await response.ToResult<IEnumerable<CategoriaResponse>>();
        }

        public async Task<IResult<IEnumerable<TitulosConectadoResponse>>> GetAllTitulosConectado()
        {
            var query = new GetAllTitulosConectadoQuery();

            var url = TitulosController.GetAllTitulosConectado;

            var response = await _httpClient.PostAsJsonAsync(url, query);

            return await response.ToResult<IEnumerable<TitulosConectadoResponse>>();
        }

        public async Task<IResult<IEnumerable<TitulosDesconectadoResponse>>> GetAllTitulosDesconectado()
        {
            var query = new GetAllTitulosDesconectadoQuery();

            var url = TitulosController.GetAllTitulosDesconectado;

            var response = await _httpClient.PostAsJsonAsync(url, query);

            return await response.ToResult<IEnumerable<TitulosDesconectadoResponse>>();
        }

        public async Task<IResult<BarraHerramientasDesconectadoResponse>> LoadDesconectado()
        {
            var response = await _httpClient.GetAsync(TitulosController.GetBarraHerramientasDesconectado);
            return await response.ToResult<BarraHerramientasDesconectadoResponse>();
        }
    }
}
