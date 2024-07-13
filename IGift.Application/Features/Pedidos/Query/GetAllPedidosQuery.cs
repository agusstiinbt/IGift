using AutoMapper;
using IGift.Application.Extensions;
using IGift.Application.Interfaces.Repositories;
using IGift.Domain.Entities;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.Features.Pedidos.Query
{
    public record GetAllPedidosQuery : IRequest<IResult<List<Domain.Entities.Pedidos>>>;

    internal class GetAllPedidosQueryHandler : IRequestHandler<GetAllPedidosQuery, IResult<List<Domain.Entities.Pedidos>>>
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPedidosQueryHandler(IUnitOfWork<string> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<List<Domain.Entities.Pedidos>>> Handle(GetAllPedidosQuery request, CancellationToken cancellationToken)
        {
            //var response = await _unitOfWork.Repository<IGift.Domain.Entities.Pedidos>().Entities.ToPaginatedListAsync(request)

            //return await Result<List<Domain.Entities.Pedidos>>.SuccessAsync(response);
            throw new NotImplementedException();
        }
    }
}
