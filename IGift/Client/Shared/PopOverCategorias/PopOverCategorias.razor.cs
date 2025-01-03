using IGift.Application.Responses.Titulos.Categoria;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Shared.PopOverCategorias
{
    public partial class PopOverCategorias
    {
        private bool _isPopoverOpen = false;

        private void ShowPopover() =>
            _isPopoverOpen = !_isPopoverOpen;

        [Parameter]
        public List<CategoriaResponse> categoriaList { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //var response = await _titulosService.GetAllCategorias();
            //if (response.Succeeded)
            //    categoriaList = response.Data.ToList();
        }
    }
}
