using Client.Infrastructure.Authentication;
using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Conectado;
using IGift.Client.Extensions;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class BarraHerramientasConectado
    {

        private List<TitulosConectadoResponse> titulosConectado = new List<TitulosConectadoResponse>();
        private List<CategoriaResponse> listaCategorias = new List<CategoriaResponse>();

        public string userName { get; set; }
        private string href { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var response = await _titulosService.LoadConectado();
            if (response.Succeeded)
            {
                titulosConectado = response.Data.Titulos.ToList();
                listaCategorias = response.Data.Categorias.ToList();

                var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
                userName = state.User.GetFirstName();
            }
        }

        public bool _open;

        public void ToggleOpen()
        {
            if (_open)
                _open = false;
            else
                _open = true;
        }

        private string MenuStyle = "visibility:collapse";
        private void ShowMenu()
        {
            if (!string.IsNullOrEmpty(MenuStyle))
            {
                MenuStyle = string.Empty;
            }
            else
            {
                MenuStyle = "visibility:collapse";
            }
        }

        bool _expanded;
    }
}
