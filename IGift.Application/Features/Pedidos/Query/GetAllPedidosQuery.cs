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
            //ReadMe and DoNotDeleteme:
            //Esto evita el uso del Mapper?Sí, esta técnica puede mejorar el rendimiento de la consulta, ya que se seleccionan y se transfieren solo los datos necesarios desde la base de datos hasta la aplicación. Además, reduce el tráfico de red y la carga en el servidor de base de datos al seleccionar solo los campos requeridos.
            Expression<Func<Domain.Entities.Peticiones, PedidosResponse>> expression = e => new PedidosResponse
            {
                Descripcion = e.Descripcion,
                Moneda = e.Moneda,
                Monto = e.Monto,
            };

            var filtro = new PedidosFilter(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var response = await _unitOfWork.Repository<Domain.Entities.Peticiones>().Entities
                    .Specify(filtro)
                    .Select(expression)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return response;
            }

            else
            {
                var ordering = string.Join(",", request.OrderBy);//TODO explicar...
                var data = await _unitOfWork.Repository<Domain.Entities.Peticiones>().Entities
                    .Specify(filtro)
                    .OrderBy(ordering)
                    .Select(expression).
                    ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }

        }
    }
}
