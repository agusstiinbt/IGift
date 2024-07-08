using AutoMapper;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Models;
using IGift.Application.Responses.Chat;
using IGift.Shared.Wrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IGift.Application.MediatR
{
    public class GetAllNotificationQuery : IRequest<List<NotificationResponse>>
    {
        public string IdUser { get; set; } = string.Empty;

        public GetAllNotificationQuery(string idUser)
        {
            IdUser = idUser;
        }

    }

    internal class GetAllProductsQueryHandler : IRequestHandler<GetAllNotificationQuery, List<NotificationResponse>>
    {
        //Acá ponemos un int porque sabemos que nuestras notificaciones tienen un Int
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

            var result = _mapper.Map<List<NotificationResponse>>(response);

            return await Result<List<NotificationResponse>>.SuccessAsync(result);
        }

        Task<List<NotificationResponse>> IRequestHandler<GetAllNotificationQuery, List<NotificationResponse>>.Handle(GetAllNotificationQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}