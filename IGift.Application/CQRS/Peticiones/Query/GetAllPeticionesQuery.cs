using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using AutoMapper;
using IGift.Application.Extensions;
using IGift.Application.Filtros.Pedidos;
using IGift.Application.Interfaces.Repositories.Generic.Auditable;
using IGift.Application.Responses.Pedidos;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.CQRS.Peticiones.Query
{
    public class GetAllPeticionesQuery : IRequest<IResult<PaginatedResult<PeticionesResponse>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
    }

    internal class GetAllPeticionesQueryHandler : IRequestHandler<GetAllPeticionesQuery, IResult<PaginatedResult<PeticionesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPeticionesQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<PaginatedResult<PeticionesResponse>>> Handle(GetAllPeticionesQuery request, CancellationToken cancellationToken)
        {
            //ReadMe and DoNotDeleteme:
            //Esto evita el uso del Mapper?Sí, esta técnica puede mejorar el rendimiento de la consulta, ya que se seleccionan y se transfieren solo los datos necesarios desde la base de datos hasta la aplicación. Además, reduce el tráfico de red y la carga en el servidor de base de datos al seleccionar solo los campos requeridos.
            Expression<Func<Domain.Entities.Peticiones, PeticionesResponse>> expression = e => new PeticionesResponse
            {
                Descripcion = e.Descripcion,
                Moneda = e.Moneda,
                Monto = e.Monto,
            };

            var filtro = new PeticionesFilter(request.SearchString);
            var response = await _unitOfWork.Repository<Domain.Entities.Peticiones>().Entities
                .Specify(filtro)
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return await Result<PaginatedResult<PeticionesResponse>>.SuccessAsync(response);
        }
    }
}
