using AutoMapper;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Application.Models.MongoDBModels.Titulos;
using IGift.Application.Models.SQL.MySQL;
using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Conectado;
using IGift.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IGift.Application.CQRS.Titulos.Conectado
{
    public record GetBarraConectadoQuery : IRequest<IResult<BarraHerramientasConectadoResponse>>;

    internal class GetBarraDesconectadoQueryHandler : IRequestHandler<GetBarraConectadoQuery, IResult<BarraHerramientasConectadoResponse>>
    {
        private readonly INonAuditableUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetBarraDesconectadoQueryHandler(INonAuditableUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<BarraHerramientasConectadoResponse>> Handle(GetBarraConectadoQuery request, CancellationToken cancellationToken)
        {
            var titulos = await _unitOfWork.Repository<TitulosConectado>().GetAllMapAsyncQuery<TitulosConectadoResponse>(_mapper);

            var categorias = await _unitOfWork.Repository<Category>().GetAllMapAsyncQuery<CategoriaResponse>(_mapper);

            var response = new BarraHerramientasConectadoResponse()
            {
                Titulos = await titulos.ToListAsync(),
                Categorias = await categorias.ToListAsync(),
            };
            return await Result<BarraHerramientasConectadoResponse>.SuccessAsync(response);
        }
    }
}
