using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IGift.Application.CQRS.Titulos.Titulos.Desconectado.Query;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Application.Responses.Titulos.Desconectado;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.CQRS.Titulos.Titulos.Conectado.Query
{
    public record GetAllTitulosConectadoQuery : IRequest<IResult<IEnumerable<TitulosDesconectadoResponse>>>;

    internal class GetAllTitulosDesconectadoQueryHandler : IRequestHandler<GetAllTitulosDesconectadoQuery, IResult<IEnumerable<TitulosDesconectadoResponse>>>
    {
        private readonly INonAuditableUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllTitulosDesconectadoQueryHandler(INonAuditableUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<IEnumerable<TitulosDesconectadoResponse>>> Handle(GetAllTitulosDesconectadoQuery request, CancellationToken cancellationToken)
        {
            var response = await _unitOfWork.Repository<Models.Titulos.TitulosDesconectado>().GetAllMapAsync<TitulosDesconectadoResponse>(_mapper);
            return await Result<IEnumerable<TitulosDesconectadoResponse>>.SuccessAsync(response);
        }
    }
}
