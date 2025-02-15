using IGift.Application.Responses.Peticiones;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.CarritoDeCompras
{
    public interface IShopCart
    {
        /// <summary>
        /// Guarda las peticiones a comprar en el carrito de compras del local storage. No acepta duplicados
        /// </summary>
        /// <returns></returns>
        Task<IResult> SaveShopCartAsync(PeticionesResponse p);

        /// <summary>
        /// Devuelve la lista de peticiones guardadas en el carrito de compras
        /// </summary>
        /// <returns></returns>
        Task<IResult<List<PeticionesResponse>>> GetShopCartAsync();

        Task ClearCarritoDeCompras();
    }
}
