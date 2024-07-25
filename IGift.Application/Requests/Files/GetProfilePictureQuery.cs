using AutoMapper;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Responses.Files;
using IGift.Shared.Wrapper;
using MediatR;

namespace IGift.Application.Requests.Files
{
    public class GetProfilePictureQuery : IRequest<IResult<ProfilePictureResponse>>
    {
        /// <summary>
        /// Id del usuario que deseamos traer la foto de perfil
        /// </summary>
        public string IdUser { get; set; } = string.Empty;
    }

    internal class GetProfilePictureQueryHandler : IRequestHandler<GetProfilePictureQuery, IResult<ProfilePictureResponse>>
    {
        private readonly IUnitOfWork<string> _unitOfWork;

        public GetProfilePictureQueryHandler(IUnitOfWork<string> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IResult<ProfilePictureResponse>> Handle(GetProfilePictureQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
