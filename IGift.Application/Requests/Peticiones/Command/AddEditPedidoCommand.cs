using AutoMapper;
using IGift.Application.Interfaces.Repositories;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.Requests.Peticiones.Command
{
    public class AddEditPeticionesCommand : IRequest<IResult>
    {
        /// <summary>
        /// Si se envía igual a 0(cero) significa que estamos editando un registro
        /// </summary>

        public string? Id { get; set; }
        public string IdUser { get; set; }
        public string Descripcion { get; set; }
        public int Monto { get; set; }
        public string Moneda { get; set; }

    }
    internal class AddPedidoCommandHandler : IRequestHandler<AddEditPeticionesCommand, IResult>
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly IMapper _mapper;

        public AddPedidoCommandHandler(IUnitOfWork<string> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult> Handle(AddEditPeticionesCommand request, CancellationToken cancellationToken)
        {
            var pedido = new Domain.Entities.Peticiones//TODO fijarse si se puede mapear
            {
                IdUser = request.IdUser,
                Descripcion = request.Descripcion,
                Moneda = request.Moneda,
                Monto = request.Monto,
                CreatedBy = "Agustin Esposito",
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now
            };
            if (string.IsNullOrEmpty(request.Id))
            {
                await _unitOfWork.Repository<Domain.Entities.Peticiones>().AddAsync(pedido);
                return await _unitOfWork.Commit("Pedido agregado con éxito", cancellationToken);
            }
            else
            {
                var peticion = await _unitOfWork.Repository<Domain.Entities.Peticiones>().GetByIdAsync(request.Id);
                if (peticion != null)
                {
                    await _unitOfWork.Repository<Domain.Entities.Peticiones>().UpdateAsync(pedido);
                    return await Result.SuccessAsync("Pedido modificado con éxito");
                }
                return await Result.FailAsync("pedido no encontrado");
            }
        }
    }
}
