using AutoMapper;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Responses.LocalAdherido;
using IGift.Domain.Entities;
using IGift.Shared.Wrapper;
using MediatR;
using System.Linq.Expressions;
using IGift.Application.Filtros.Locales;
using IGift.Application.Extensions;

namespace IGift.Application.Features.LocalesAdheridos.Query
{
    public class GetAllLocalAdheridoQuery : IRequest<IResult<IEnumerable<LocalAdheridoResponse>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }//TODO documentar que en el front cuando usamos un tablestate siempre tiene por defecto un 10. No hay que usar en el front un pagination, sino que debemos usar un MudTablePager porque este otorga siempre por defecto 10. Ver el código de Blazor Hero la parte de Products para ver cómo se comporta    
        public string SearchString { get; set; }

        public GetAllLocalAdheridoQuery(int pageNumber, int pageSize, string searchString)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
        }
    }
    internal class GetAllLocalAdheridoQueryHandler : IRequestHandler<GetAllLocalAdheridoQuery, IResult<IEnumerable<LocalAdheridoResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllLocalAdheridoQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<IEnumerable<LocalAdheridoResponse>>> Handle(GetAllLocalAdheridoQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<LocalAdherido, LocalAdheridoResponse>> expression = e => new LocalAdheridoResponse
            {
                Descripcion = e.Descripcion,
                ImageDataURL = e.ImageDataURL,
                Nombre = e.Nombre,
            };

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var filtro = new LocalesFilter(request.SearchString);
                var response = await _unitOfWork.Repository<LocalAdherido>().Entities.Specify(filtro).Select(expression).ToPaginatedListAsync();
            }
            throw new NotImplementedException();
        }
    }
}
