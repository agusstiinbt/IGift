using AutoMapper;
using IGift.Application.Enums;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Application.Models.MongoDBModels;
using IGift.Application.Responses.Notification;
using IGift.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IGift.Application.CQRS.Notifications.Query
{
    public class GetAllNotificationQuery : IRequest<IResult<IEnumerable<NotificationResponse>>>
    {
        public string IdUser { get; set; } = string.Empty;

        public DateTime? FechaHasta { get; set; } = null;

        public TypeNotification? TypeNotification { get; set; } = null;
    }

    internal class GetAllNotificationQueryHandler : IRequestHandler<GetAllNotificationQuery, IResult<IEnumerable<NotificationResponse>>>
    {
        //Acá ponemos un int porque sabemos que nuestras notificaciones tienen un Int
        private readonly INonAuditableUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllNotificationQueryHandler(INonAuditableUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<IEnumerable<NotificationResponse>>> Handle(GetAllNotificationQuery request, CancellationToken cancellationToken)
        {
            var query = await _unitOfWork.Repository<Notification>().GetAllMapAsyncQuery<NotificationResponse>(_mapper);

            if (!string.IsNullOrEmpty(request.IdUser))//Traemos solamente aquellas notificaciones que corresponda al IdUser pasado
                query = query.Where(x => x.IdUser == request.IdUser);

            if (request.FechaHasta != null)
                query = query.Where(x => x.DateTime > request.FechaHasta);

            if (request.TypeNotification != null)
                query = query.Where(x => x.Type == request.TypeNotification);

            return await Result<IEnumerable<NotificationResponse>>.SuccessAsync(await query.ToListAsync());
        }
    }
}