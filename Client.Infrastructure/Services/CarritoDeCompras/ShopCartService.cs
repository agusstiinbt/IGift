﻿using System.Text.Json;
using Blazored.LocalStorage;
using IGift.Application.Responses.Peticiones;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using Microsoft.JSInterop;

namespace IGift.Client.Infrastructure.Services.CarritoDeCompras
{
    public class ShopCartService : IShopCart
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IJSRuntime _js;

        public ShopCartService(ILocalStorageService localStorage, IJSRuntime js)
        {
            _localStorage = localStorage;
            _js = js;
        }

        public async Task<IResult> SaveShopCartAsync(PeticionesResponse p)
        {
            var carrito = await GetPeticiones();

            if (!carrito.Any(x => x.Id == p.Id))
            {
                carrito!.Add(p);
                var json = JsonSerializer.Serialize(carrito);

                await _localStorage.SetItemAsync(AppConstants.Local.ShopCart, json);

                return await Result.SuccessAsync();
            }
            return await Result.FailAsync("Ya se posee esa petición en el carrito de compras");
        }

        public async Task<IResult<List<PeticionesResponse>>> GetShopCartAsync()
        {
            var response = await GetPeticiones();

            return await Result<List<PeticionesResponse>>.SuccessAsync(response);
        }

        /// <summary>
        /// Lee la lista existene de 'carrito' en el local storage. Si hay algo en 'carrito' devuelve una lista deserializada. Si esta vacío, devuelve una lista nueva.
        /// </summary>
        /// <returns>Una lista de AddPeticionesCommand</returns>
        private async Task<List<PeticionesResponse>> GetPeticiones()
        {
            var json = await _localStorage.GetItemAsync<string>(AppConstants.Local.ShopCart);

            List<PeticionesResponse> carrito;

            if (!string.IsNullOrEmpty(json))
            {
                // Si hay algo en 'carrito', deserializarlo a una lista 
                carrito = JsonSerializer.Deserialize<List<PeticionesResponse>>(json);
            }
            else
            {
                // Si 'carrito' está vacío, inicializar una nueva lista
                carrito = new List<PeticionesResponse>();
            }

            return carrito!;
            // Leer la lista existente de 'carrito'
        }

        public async Task ClearCache()
        {
            await _localStorage.ClearAsync();
        }
    }
}
