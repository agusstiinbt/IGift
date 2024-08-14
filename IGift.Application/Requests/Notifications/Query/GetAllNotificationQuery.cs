using AutoMapper;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Models;
using IGift.Application.Responses.Notification;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.Requests.Notifications.Query
{
    public record GetAllNotificationQuery(string Id) : IRequest<IResult<IEnumerable<NotificationResponse>>>;

    internal class GetAllNotificationQueryHandler : IRequestHandler<GetAllNotificationQuery, IResult<IEnumerable<NotificationResponse>>>
    {
        //Acá ponemos un int porque sabemos que nuestras notificaciones tienen un Int
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllNotificationQueryHandler(IUnitOfWork<string> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<IEnumerable<NotificationResponse>>> Handle(GetAllNotificationQuery request, CancellationToken cancellationToken)
        {
            var response = await _unitOfWork.Repository<Notification>().GetAllAsync();
            var lista = new List<NotificationResponse>();
            //TODO modificar y hacer algun tipo de mapeo
            foreach (var item in response)
            {
                lista.Add(new NotificationResponse
                {
                    DateTime = item.DateTime,
                    Message = item.Message,
                    Type = item.Type
                });
            }

            //TODO implementar el mapeo aquí
            //var result = _mapper.Map<List<NotificationResponse>>(response);

            return await Result<IEnumerable<NotificationResponse>>.SuccessAsync(lista);
        }
    }
}