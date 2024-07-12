using AutoMapper;
using IGift.Application.Interfaces.Repositories;
using IGift.Domain.Entities;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.Features.Pedidos.Query
{
    public record GetAllPedidosQuery : IRequest<IResult<List<Request>>>;

    internal class GetAllPedidosQueryHandler : IRequestHandler<GetAllPedidosQuery, IResult<List<Request>>>
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPedidosQueryHandler(IUnitOfWork<string> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<List<Request>>> Handle(GetAllPedidosQuery request, CancellationToken cancellationToken)
        {
            var response = await _unitOfWork.Repository<Request>().GetAllAsync();

            return await Result<List<Request>>.SuccessAsync(response);
        }
    }
}
