using Client.Infrastructure.Services.Requests;
using IGift.Application.Features.Pedidos.Query;
using IGift.Application.Responses.Pedidos;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Pages
{
    public partial class Peticiones
    {
        [Inject] IPeticionesService _peticiones { get; set; }

        private IEnumerable<PeticionesResponse> _pagedData;
        private MudTable<PeticionesResponse> _table;

        private int _totalItems;
        private string _searchString { get; set; } = string.Empty;
        private int _currentPage;


        private async Task<TableData<PeticionesResponse>> GetData(TableState state)
        {
            if (!string.IsNullOrEmpty(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<PeticionesResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            //TODO implementar el ordenamiento
            // public GetAllPeticionesQuery(int pageNumber, int pageSize, string searchString, string[] orderBy)

            var request = new GetAllPeticionesQuery { PageNumber = pageNumber, PageSize = pageSize, SearchString = _searchString };
            var response = await _peticiones.GetAll(request);
            if (response.Succeeded)
            {
                _totalItems = response.Data.TotalCount;
                _currentPage = response.Data.CurrentPage;
                _pagedData = response.Data.Data;
            }
            else
            {
                foreach (var messages in response.Messages)
                {
                    _snack.Add(messages, Severity.Error);
                }
            }
        }

        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }
    }
}
