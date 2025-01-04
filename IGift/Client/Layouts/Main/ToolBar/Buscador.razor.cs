using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Responses.Peticiones;
using IGift.Domain.Entities;
using IGift.Shared.Wrapper;
using MudBlazor;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class Buscador
    {
        public string Search { get; set; } = string.Empty;

        public async Task<PaginatedResult<PeticionesResponse>> RealizarBusqueda()
        {
            var query = new GetAllPeticionesQuery() { SearchString = Search };

            var response = await _peticionesService.GetAll(query);

            if (!response.Succeeded)
            {
                _snack.Add(response.Messages.FirstOrDefault(), Severity.Warning);
            }
            return response.Data;
        }
    }
}
