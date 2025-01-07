using System.Linq.Expressions;
using AutoMapper;
using IGift.Application.Extensions;
using IGift.Application.Filtros.Pedidos;
using IGift.Application.Interfaces.Repositories.Generic.Auditable;
using IGift.Application.Responses.Peticiones;
using IGift.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IGift.Application.CQRS.Peticiones.Query
{
    /// <summary>
    /// Con esta clase podemos buscar peticiones segun las propiedades que le carguemos. Si vamos a hacer una busqueda con un resultado "Similar" entonces utilizar SearchString. Si utilizamos una busqueda de tipo exacta, utilizamos Descripcion y/o Categoria
    /// </summary>
    public class GetAllPeticionesQuery : IRequest<PaginatedResult<PeticionesResponse>>
    {
        /// <summary>
        /// Util para busqueda que contenga resultados similares
        /// </summary>
        public string? SearchString { get; set; } = string.Empty;
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

    internal class GetAllPeticionesQueryHandler : IRequestHandler<GetAllPeticionesQuery, PaginatedResult<PeticionesResponse>>
    {
        private readonly IAuditableUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPeticionesQueryHandler(IAuditableUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<PeticionesResponse>> Handle(GetAllPeticionesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = await _unitOfWork.Repository<Domain.Entities.Peticiones>().FindAndMapByQuery<PeticionesResponse>(_mapper);
                if (!string.IsNullOrEmpty(request.SearchString))
                {

                    Expression<Func<Domain.Entities.Peticiones, PeticionesResponse>> expression = e => new PeticionesResponse
                    {
                        Descripcion = e.Descripcion,
                        Moneda = e.Moneda,
                        Monto = e.Monto,
                    };

                    var filtro = new PeticionesFilter(request.SearchString);

                    var response1 = await _unitOfWork.Repository<Domain.Entities.Peticiones>().Query
                   .Specify(filtro)
                    .Select(expression)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                    if (response1.Data.Count > 0)
                    {
                        return response1;
                    }
                }

                if (!string.IsNullOrEmpty(request.Descripcion))
                    query = query.Where(x => x.Descripcion == request.Descripcion);

                if (!string.IsNullOrEmpty(request.Moneda))
                    query = query.Where(x => x.Moneda == request.Moneda);

                if (request.Monto != 0)
                    query = query.Where(x => x.Monto == request.Monto);

                var response = await query.ToPaginatedListAsync(0, 0);

                return response;
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}
