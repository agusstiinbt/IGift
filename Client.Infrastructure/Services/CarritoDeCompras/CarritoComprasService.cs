﻿using IGift.Application.Responses.Pedidos;
using IGift.Shared.Wrapper;
using IGift.Application.Requests.Peticiones.Command;
using IGift.Shared;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System.Text.Json;

namespace IGift.Client.Infrastructure.Services.CarritoDeCompras
{
    public class CarritoComprasService : ICarritoComprasService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IJSRuntime _js;

        public CarritoComprasService(ILocalStorageService localStorage, IJSRuntime js)
        {
            _localStorage = localStorage;
            _js = js;
        }

        public async Task<IResult> GuardarEnCarritoDeCompras(PeticionesResponse p)
        {
            var carrito = await GetPeticiones();

            var idUser = await _localStorage.GetItemAsync<string>(AppConstants.StorageConstants.Local.IdUser);
            var command = new AddEditPeticionesCommand()
            {
                Id = string.Empty,
                IdUser = idUser!,
                Descripcion = p.Descripcion,
                Monto = p.Monto,
                Moneda = p.Moneda
            };

            carrito!.Add(command);

            var json = JsonSerializer.Serialize(carrito);

            await _localStorage.SetItemAsync(AppConstants.StorageConstants.Local.ShopCart, json);

            return await Result.SuccessAsync();
        }

        public async Task<IResult<List<AddEditPeticionesCommand>>> ObtenerCarritoDePeticiones()
        {
            var response = await GetPeticiones();

            return await Result<List<AddEditPeticionesCommand>>.SuccessAsync(response);
        }

        /// <summary>
        /// Lee la lista existene de 'carrito' en el local storage. Si hay algo en 'carrito' devuelve una lista deserializada. Si esta vacío, devuelve una lista nueva.
        /// </summary>
        /// <returns>Una lista de AddPeticionesCommand</returns>
        private async Task<List<AddEditPeticionesCommand>> GetPeticiones()
        {
            // Leer la lista existente de 'carrito'
            var json = await _localStorage.GetItemAsync<string>(AppConstants.StorageConstants.Local.ShopCart);

            List<AddEditPeticionesCommand> carrito;

            if (!string.IsNullOrEmpty(json))
            {
                // Si hay algo en 'carrito', deserializarlo a una lista 
                carrito = JsonSerializer.Deserialize<List<AddEditPeticionesCommand>>(json);
            }
            else
            {
                // Si 'carrito' está vacío, inicializar una nueva lista
                carrito = new List<AddEditPeticionesCommand>();
            }

            return carrito!;
        }
    }
}
