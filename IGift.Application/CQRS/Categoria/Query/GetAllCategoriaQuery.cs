using AutoMapper;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Application.Responses.Categoria;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.CQRS.Categoria.Query
{
    public record GetAllCategoriaQuery : IRequest<IResult<IEnumerable<CategoriaResponse>>>;

    internal class GetAllCategoriaQueryHandler : IRequestHandler<GetAllCategoriaQuery, IResult<IEnumerable<CategoriaResponse>>>
    {
        private readonly INonAuditableUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCategoriaQueryHandler(INonAuditableUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<IEnumerable<CategoriaResponse>>> Handle(GetAllCategoriaQuery request, CancellationToken cancellationToken)
        {
            var response = await _unitOfWork.Repository<Models.Categoria>().GetAllMapAsync<CategoriaResponse>(_mapper);
            return await Result<IEnumerable<CategoriaResponse>>.SuccessAsync(response);
        }
    }
}
