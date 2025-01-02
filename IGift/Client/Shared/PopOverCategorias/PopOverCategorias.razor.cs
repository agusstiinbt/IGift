using IGift.Application.Responses.Titulos.Categoria;

namespace IGift.Client.Shared.PopOverCategorias
{
    public partial class PopOverCategorias
    {

        private bool _isPopoverOpen = false;

        private void ShowPopover() =>
            _isPopoverOpen = !_isPopoverOpen;

        private List<CategoriaResponse> categoriaList = new List<CategoriaResponse>();

        protected override async Task OnInitializedAsync()
        {
            var response = await _titulosService.GetAllCategorias();
            if (response.Succeeded)
                categoriaList = response.Data.ToList();
        }
    }
}
