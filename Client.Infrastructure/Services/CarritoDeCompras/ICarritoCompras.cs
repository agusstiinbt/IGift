﻿using IGift.Application.Requests.Peticiones.Command;
using IGift.Application.Responses.Pedidos;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.CarritoDeCompras
{
    public interface ICarritoComprasService
    {
        /// <summary>
        /// Guarda las peticiones a comprar en el carrito de compras del local storage
        /// </summary>
        /// <returns></returns>
        Task<IResult> GuardarEnCarritoDeCompras(PeticionesResponse p);

        /// <summary>
        /// Devuelve la lista de peticiones guardadas en el carrito de compras
        /// </summary>
        /// <returns></returns>
        Task<IResult<List<AddEditPeticionesCommand>>> ObtenerCarritoDePeticiones();

    }
}
