using System.Linq.Expressions;
using AutoMapper;
using IGift.Application.Extensions;
using IGift.Application.Filtros.Pedidos;
using IGift.Application.Interfaces.Repositories.Generic.Auditable;
using IGift.Application.Responses.Peticiones;
using IGift.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Primitives;

namespace IGift.Application.CQRS.Peticiones.Query
{
    /// <summary>
    /// Con esta clase podemos buscar peticiones segun las propiedades que le carguemos. Si vamos a hacer una busqueda con un resultado "Similar" entonces utilizar SearchString. Si utilizamos una busqueda de tipo exacta, utilizamos Descripcion y/o Categoria
    /// </summary>
    public class GetAllPeticionesQuery : IRequest<IResult<PaginatedResult<PeticionesResponse>>>
    {
        /// <summary>
        /// Util para busqueda que contenga resultados similares
        /// </summary>
        public string? SearchString { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// Util para busqueda con resultados exactos
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;
        public string Moneda { get; set; } = string.Empty;
        /// <summary>
        /// Util para busqueda con resultados exactos
        /// </summary>
        public int Monto { get; set; }
        /// <summary>
        /// Util para busqueda con resultados exactos
        /// </summary>
        public string Categoria { get; set; } = string.Empty;
    }

    internal class GetAllPeticionesQueryHandler : IRequestHandler<GetAllPeticionesQuery, IResult<PaginatedResult<PeticionesResponse>>>
    {
        private readonly IAuditableUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPeticionesQueryHandler(IAuditableUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<PaginatedResult<PeticionesResponse>>> Handle(GetAllPeticionesQuery request, CancellationToken cancellationToken)
        {
            var query = await _unitOfWork.Repository<Domain.Entities.Peticiones>().FindAndMapByQuery<PeticionesResponse>(_mapper);

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var filtro = new PeticionesFilter(request.SearchString);

                Expression<Func<Domain.Entities.Peticiones, PeticionesResponse>> expression = e => new PeticionesResponse
                {
                    Descripcion = e.Descripcion,
                    Moneda = e.Moneda,
                    Monto = e.Monto,
                };

                var response1 = await _unitOfWork.Repository<Domain.Entities.Peticiones>().Query
               .Specify(filtro)
               .Select(expression)
               .ToPaginatedListAsync(request.PageNumber, request.PageSize);

                if (response1.Data.Count > 0)
                {
                    return await Result<PaginatedResult<PeticionesResponse>>.SuccessAsync(response1);
                }
                return await Result<PaginatedResult<PeticionesResponse>>.FailAsync("No se han encontrado datos");
            }

            if (!string.IsNullOrEmpty(request.Descripcion))
                query = query.Where(x => x.Descripcion == request.Descripcion);

            if (!string.IsNullOrEmpty(request.Moneda))
                query = query.Where(x => x.Moneda == request.Moneda);

            if (request.Monto != 0)
                query = query.Where(x => x.Monto == request.Monto);

            var response = await query.ToPaginatedListAsync(0, 0);

            return await Result<PaginatedResult<PeticionesResponse>>.SuccessAsync(response);


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


            //return await Result<PaginatedResult<PeticionesResponse>>.SuccessAsync(response);
        }
    }
}
