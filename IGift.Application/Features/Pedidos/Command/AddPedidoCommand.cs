using AutoMapper;
using IGift.Application.Interfaces.Repositories;
using IGift.Domain.Entities;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.Features.Pedidos.Command
{
    public class AddPedidoCommand : IRequest<IResult>
    {
        public string IdUser { get; set; }
        public string Descripcion { get; set; }
        public int Monto { get; set; }
        public required string Moneda { get; set; }

    }
    internal class AddPedidoCommandHandler : IRequestHandler<AddPedidoCommand, IResult>
    {


        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly IMapper _mapper;

        public AddPedidoCommandHandler(IUnitOfWork<string> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult> Handle(AddPedidoCommand request, CancellationToken cancellationToken)
        {
            var pedido = new Domain.Entities.Pedidos
            {
                IdUser = request.IdUser,
                Descripcion = request.Descripcion,
                Moneda = request.Moneda,
                Monto = request.Monto,
                CreatedBy = "Agustin Esposito",
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now
            };
            await _unitOfWork.Repository<Domain.Entities.Pedidos>().AddAsync(pedido);

            return await Result.SuccessAsync("Pedido agregado con éxito");
        }
    }
}
