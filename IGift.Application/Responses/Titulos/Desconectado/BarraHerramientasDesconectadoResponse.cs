using IGift.Application.Responses.Titulos.Categoria;

namespace IGift.Application.Responses.Titulos.Desconectado
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
