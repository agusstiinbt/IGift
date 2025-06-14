using IGift.Application.Enums;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Application.Models.MongoDBModels;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.CQRS.Notifications.Command
{
    public class SaveNotificationCommand : IRequest<IResult>
    {
        public string IdUser { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public TypeNotification TypeNotification { get; set; }
    }

    internal class SaveNotificationCommandHandler : IRequestHandler<SaveNotificationCommand, IResult>
    {
        private readonly INonAuditableUnitOfWork<int> _unitOfWork;

        public SaveNotificationCommandHandler(INonAuditableUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(SaveNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.Repository<Notification>().AddAsync(new Notification() { DateTime = request.DateTime, IdUser = request.IdUser, Message = request.Message, Type = request.TypeNotification });
            }
            catch (Exception e)
            {
                throw new Exception("Error al guardar la notificacion. SaveNotificationCommandHandler. Mensaje de error " + e.Message);
            }
            return await Result.SuccessAsync("Notificacion guardada con exito");
        }
    }
}
