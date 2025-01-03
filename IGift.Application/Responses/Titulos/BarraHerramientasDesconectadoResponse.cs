using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Desconectado;

namespace IGift.Application.Responses.Titulos
{
    public class BarraHerramientasDesconectadoResponse
    {
        public BarraHerramientasDesconectadoResponse()
        {
            Categorias = new List<CategoriaResponse>();
            Titulos = new List<TitulosDesconectadoResponse>();
        }

        public List<CategoriaResponse> Categorias { get; set; }
        public List<TitulosDesconectadoResponse> Titulos { get; set; }
    }
}
