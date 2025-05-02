using Client.Infrastructure.Extensions;
using IGift.Application.Responses.Titulos.Conectado;
using IGift.Application.Responses.Titulos.Desconectado;
using IGift.Shared.Constants;
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

        public async Task<IResult<BarraHerramientasConectadoResponse>> LoadConectado()
        {
            var response = await _httpClient.GetAsync(AppConstants.Controller.Titulos.GetBarraHerramientasConectado);
            return await response.ToResult<BarraHerramientasConectadoResponse>();
        }

        public async Task<IResult<BarraHerramientasDesconectadoResponse>> LoadDesconectado()
        {
            var response = await _httpClient.GetAsync(AppConstants.Controller.Titulos.GetBarraHerramientasDesconectado);
            return await response.ToResult<BarraHerramientasDesconectadoResponse>();
        }
    }
}
