using AutoMapper;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Application.Responses.Titulos.Conectado;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.CQRS.Titulos.Titulos.Conectado.Query
{
    public record GetAllTitulosConectadoQuery : IRequest<IResult<IEnumerable<TitulosConectadoResponse>>>;

    internal class GetAllTitulosConectadoHandler : IRequestHandler<GetAllTitulosConectadoQuery, IResult<IEnumerable<TitulosConectadoResponse>>>
    {
        private readonly INonAuditableUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllTitulosConectadoHandler(INonAuditableUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<IEnumerable<TitulosConectadoResponse>>> Handle(GetAllTitulosConectadoQuery request, CancellationToken cancellationToken)
        {
            var response = await _unitOfWork.Repository<Models.Titulos.TitulosConectado>().GetAllMapAsync<TitulosConectadoResponse>(_mapper);
            return await Result<IEnumerable<TitulosConectadoResponse>>.SuccessAsync(response);
        }
    }
}
