using AutoMapper;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Application.Models.MongoDBModels.Titulos;
using IGift.Application.Models.SQL.MySQL;
using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Desconectado;
using IGift.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var titulos = await _unitOfWork.Repository<TitulosDesconectado>().GetAllMapAsyncQuery<TitulosDesconectadoResponse>(_mapper);

            var categorias = await _unitOfWork.Repository<Category>().GetAllMapAsyncQuery<CategoriaResponse>(_mapper);

            var response = new BarraHerramientasDesconectadoResponse()
            {
                Titulos = await titulos.ToListAsync(),
                Categorias = await categorias.ToListAsync(),
            };

            return await Result<BarraHerramientasDesconectadoResponse>.SuccessAsync(response);
        }
    }
}
