using IGift.Application.Responses.Titulos.Categoria;
using IGift.Client.Infrastructure.Services.Categoria;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Shared.PopOverCategorias
{
    public partial class PopOverCategorias
    {
        [Inject] ICategoriaService _categoriaService { get; set; }

        private bool _isPopoverOpen = false;

        private void ShowPopover() =>
            _isPopoverOpen = !_isPopoverOpen;

        private List<CategoriaResponse> categoriaList = new List<CategoriaResponse>();

        protected override async Task OnInitializedAsync()
        {
            var response = await _categoriaService.GetAll();
            if (response.Succeeded)
                categoriaList = response.Data.ToList();
        }
    }
}
