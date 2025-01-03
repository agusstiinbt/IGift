using AutoMapper;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Application.Responses.Titulos;
using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Desconectado;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.CQRS.Titulos.Desconectado
{
    public record GetBarraDesconectadoQuery : IRequest<IResult<BarraHerramientasDesconectadoResponse>>;

    internal class GetBarraDesconectadoQueryHandler : IRequestHandler<GetBarraDesconectadoQuery, IResult<BarraHerramientasDesconectadoResponse>>
    {
        private readonly INonAuditableUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetBarraDesconectadoQueryHandler(INonAuditableUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<BarraHerramientasDesconectadoResponse>> Handle(GetBarraDesconectadoQuery request, CancellationToken cancellationToken)
        {
            var titulos = await _unitOfWork.Repository<Models.Titulos.TitulosDesconectado>().GetAllMapAsync<TitulosDesconectadoResponse>(_mapper);

            var categorias = await _unitOfWork.Repository<Models.Titulos.Categoria>().GetAllMapAsync<CategoriaResponse>(_mapper);

            var response = new BarraHerramientasDesconectadoResponse()
            {
                Titulos = titulos.ToList(),
                Categorias = categorias.ToList(),
            };

            return await Result<BarraHerramientasDesconectadoResponse>.SuccessAsync(response);
        }
    }
}
