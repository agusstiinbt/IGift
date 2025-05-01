using System.Linq.Expressions;
using AutoMapper;
using IGift.Application.Extensions;
using IGift.Application.Filtros.Locales;
using IGift.Application.Interfaces.Repositories.Generic.Auditable;
using IGift.Application.Responses.LocalAdherido;
using IGift.Domain.Entities;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.CQRS.LocalesAdheridos.Query
{
    public class GetAllLocalAdheridoQuery : IRequest<IResult<PaginatedResult<LocalAdheridoResponse>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }//En el front cuando usamos un tablestate siempre tiene por defecto un 10. No hay que usar en el front un pagination, sino que debemos usar un MudTablePager porque este otorga siempre por defecto 10. Ver el código de Blazor Hero la parte de Products para ver cómo se comporta    
        public string SearchString { get; set; }

        public GetAllLocalAdheridoQuery(int pageNumber, int pageSize, string searchString)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
        }
    }
    internal class GetAllLocalAdheridoQueryHandler : IRequestHandler<GetAllLocalAdheridoQuery, IResult<PaginatedResult<LocalAdheridoResponse>>>
    {
        private readonly IAuditableUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllLocalAdheridoQueryHandler(IAuditableUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<PaginatedResult<LocalAdheridoResponse>>> Handle(GetAllLocalAdheridoQuery request, CancellationToken cancellationToken)
        {
            //Esto evita un mapeo, generando menos tráfico de datos por parte del servidor
            Expression<Func<LocalAdherido, LocalAdheridoResponse>> expression = e => new LocalAdheridoResponse
            {
                Descripcion = e.Descripcion,
                ImageDataURL = e.ImageDataURL,
                Nombre = e.Nombre,
            };

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var filtro = new LocalesFilter(request.SearchString);
                var response = await _unitOfWork.Repository<LocalAdherido>().Query.Specify(filtro).Select(expression).ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return await Result<PaginatedResult<LocalAdheridoResponse>>.SuccessAsync(response);
            }
            else
            {
                var response = await _unitOfWork.Repository<LocalAdherido>().Query.Select(expression).ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return await Result<PaginatedResult<LocalAdheridoResponse>>.SuccessAsync(response);
            }
        }
    }
}
