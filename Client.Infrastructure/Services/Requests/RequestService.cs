﻿using Client.Infrastructure.Extensions;
using IGift.Application.Features.Pedidos.Query;
using IGift.Application.Responses.Pedidos;
using IGift.Shared;
using IGift.Shared.Wrapper;
using System.Net.Http.Json;

namespace Client.Infrastructure.Services.Requests
{
    public class PedidosService : IPedidosService
    {
        private readonly HttpClient _http;

        public PedidosService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PaginatedResult<PedidosResponse>> GetAll()
        {
            //TODO va a llegar un momento donde vamos a tener mil productos, entonces ver el ejemplo de blazorHero
            var response = await _http.PostAsJsonAsync(AppConstants.Controllers.PedidosController.GetAll, new GetAllPedidosQuery());
            return await response.ToPaginatedResult<PedidosResponse>();
        }
    }
}
