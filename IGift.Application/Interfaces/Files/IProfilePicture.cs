using IGift.Application.CQRS.Files.ProfilePicture;
using IGift.Application.Responses.Files;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Files
{
    public interface IProfilePicture
    {
        Task<IResult<ProfilePictureResponse>> GetByUserIdAsync(string IdUser);
        Task<IResult> SaveProfilePictureAsync(ProfilePictureUpload request);

        /// <summary>
        /// Este metodo debe ser usado solamente dentro del mismo contexto. Para enviar datos a traves de una api utilizasr el otro
        /// </summary>
        /// <param name="IdUser"></param>
        /// <returns></returns>
        Task<ProfilePictureResponse> GetByUserIdAsync2(string IdUser);
    }
}
