using AutoMapper;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Models;
using IGift.Application.Responses.Chat;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.Features.Notifications.Query
{
    public record GetAllNotificationQuery(string id) : IRequest<IResult<List<NotificationResponse>>>;

    internal class GetAllProductsQueryHandler : IRequestHandler<GetAllNotificationQuery, IResult<List<NotificationResponse>>>
    {
        //Acá ponemos un int porque sabemos que nuestras notificaciones tienen un GUID
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(IUnitOfWork<Guid> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<List<NotificationResponse>>> Handle(GetAllNotificationQuery request, CancellationToken cancellationToken)
        {
            var response = await _unitOfWork.Repository<Notification>().GetAllAsync();

            var lista= response.Where(x=>x.IdUser==request.id).ToList();

            var result = _mapper.Map<List<NotificationResponse>>(response);

            return await Result<List<NotificationResponse>>.SuccessAsync(result);
        }
    }
}