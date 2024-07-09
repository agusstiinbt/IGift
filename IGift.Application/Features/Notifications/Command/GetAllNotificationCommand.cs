using MediatR;

namespace IGift.Application.Features.Notifications.Command
{
    public record GetAllNotificationCommand(string id) : IRequest<int>;

    internal class GetAllNotificationCommandHandler : IRequestHandler<GetAllNotificationCommand, int>
    {
        public Task<int> Handle(GetAllNotificationCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
