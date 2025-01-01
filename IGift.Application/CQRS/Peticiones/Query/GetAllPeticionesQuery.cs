using AutoMapper;
using IGift.Application.Interfaces.Repositories.Generic.Auditable;
using IGift.Application.Responses.Peticiones;
using IGift.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IGift.Application.CQRS.Peticiones.Query
{
    /// <summary>
    /// Con esta clase podemos buscar peticiones segun las propiedades que le carguemos
    /// </summary>
    public class GetAllPeticionesQuery : IRequest<IResult<IEnumerable<PeticionesResponse>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Moneda { get; set; } = string.Empty;
        public int Monto { get; set; }
        public string Categoria { get; set; } = string.Empty;
    }

    internal class GetAllPeticionesQueryHandler : IRequestHandler<GetAllPeticionesQuery, IResult<IEnumerable<PeticionesResponse>>>
    {
        private readonly IAuditableUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPeticionesQueryHandler(IAuditableUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<IEnumerable<PeticionesResponse>>> Handle(GetAllPeticionesQuery request, CancellationToken cancellationToken)
        {
            var query = await _unitOfWork.Repository<Domain.Entities.Peticiones>().FindAndMapByQuery<PeticionesResponse>(_mapper);

            if (!string.IsNullOrEmpty(request.Descripcion))
                query = query.Where(x => x.Descripcion == request.Descripcion);

            if (!string.IsNullOrEmpty(request.Moneda))
                query = query.Where(x => x.Moneda == request.Moneda);

            if (request.Monto != 0)
                query = query.Where(x => x.Monto == request.Monto);

            var response = await query.ToListAsync();
            return await Result<IEnumerable<PeticionesResponse>>.SuccessAsync(response);


            //IEnumerable<Domain.Entities.Peticiones> response;
            //if (!string.IsNullOrEmpty(request.Categoria))
            //{
            //    response = await _unitOfWork.Repository<Domain.Entities.Peticiones>().FindAsync(x => x.Categoria == request.Categoria);
            //}

            //response = await _unitOfWork.Repository<Domain.Entities.Peticiones>().GetAllAsync();


            //ReadMe and DoNotDeleteme:
            //Esto evita el uso del Mapper?Sí, esta técnica puede mejorar el rendimiento de la consulta, ya que se seleccionan y se transfieren solo los datos necesarios desde la base de datos hasta la aplicación. Además, reduce el tráfico de red y la carga en el servidor de base de datos al seleccionar solo los campos requeridos.
            //Es medio confuso lo siguiente y habria que hacer una clase filter para todo lo que se te ocurra, es mejor lo de arriba por mucho
            //Expression<Func<Domain.Entities.Peticiones, PeticionesResponse>> expression = e => new PeticionesResponse
            //{
            //    Descripcion = e.Descripcion,
            //    Moneda = e.Moneda,
            //    Monto = e.Monto,
            //};

            //var filtro = new PeticionesFilter(request.SearchString);
            //var response = await _unitOfWork.Repository<Domain.Entities.Peticiones>().Entities
            //    .Specify(filtro)
            //    .Select(expression)
            //    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            //return await Result<PaginatedResult<PeticionesResponse>>.SuccessAsync(response);
        }
    }
}
