using AutoMapper;
using IGift.Application.Extensions;
using IGift.Application.Filtros.Pedidos;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Responses.Pedidos;
using IGift.Shared.Wrapper;
using MediatR;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace IGift.Application.Features.Pedidos.Query
{
    public class GetAllPedidosQuery : IRequest<PaginatedResult<PedidosResponse>>
    {
        public GetAllPedidosQuery(int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            OrderBy = orderBy;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; }
    }

    internal class GetAllPedidosQueryHandler : IRequestHandler<GetAllPedidosQuery, PaginatedResult<PedidosResponse>>
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPedidosQueryHandler(IUnitOfWork<string> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<PedidosResponse>> Handle(GetAllPedidosQuery request, CancellationToken cancellationToken)
        {
            //TODO esto evita el uso del Mapper?
            Expression<Func<Domain.Entities.Pedidos, PedidosResponse>> expression = e => new PedidosResponse
            {
                Descripcion = e.Descripcion,
                Moneda = e.Moneda,
                Monto = e.Monto,
            };

            var filtro = new PedidosFilter(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var response = await _unitOfWork.Repository<Domain.Entities.Pedidos>().Entities
                    .Specify(filtro)
                    .Select(expression)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return response;
            }

            else
            {
                var ordering = string.Join(",", request.OrderBy);//TODO explicar...
                var data = await _unitOfWork.Repository<Domain.Entities.Pedidos>().Entities
                    .Specify(filtro)
                    .OrderBy(ordering)
                    .Select(expression).
                    ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }

        }
    }
}
