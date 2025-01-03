using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Desconectado;
using IGift.Application.Responses.Titulos;
using IGift.Shared.Wrapper;
using MediatR;
using IGift.Application.Responses.Titulos.Conectado;

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
            var titulos = await _unitOfWork.Repository<Models.Titulos.TitulosConectado>().GetAllMapAsync<TitulosConectadoResponse>(_mapper);

            var categorias = await _unitOfWork.Repository<Models.Titulos.Categoria>().GetAllMapAsync<CategoriaResponse>(_mapper);

            var response = new BarraHerramientasConectadoResponse()
            {
                Titulos = titulos.ToList(),
                Categorias = categorias.ToList(),
            };

            return await Result<BarraHerramientasConectadoResponse>.SuccessAsync(response);
        }
    }
}
