using IGift.Application.Requests.Files.ProfilePicture;
using IGift.Application.Responses.Files;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Files
{
    /// <summary>
    /// Esta clase se encarga del CRUD de la foto de perfil de un usuario
    /// </summary>
    public interface IProfilePicture
    {
        /// <summary>
        /// Con este método podemos traer la foto del UserId como parámetro
        /// </summary>
        /// <param name="Id">Id del usuario a traer la foto</param>
        /// <returns>IResult con, en caso de éxito, el response que contiene los bytes de la imagen</returns>
        Task<IResult<ProfilePictureResponse>> GetByIdAsync(string Id);
        /// <summary>
        /// Con este método podemos cargar una nueva foto de perfil, pisando la foto anterior
        /// </summary>
        /// <param name="request">Ver dentro para más información</param>
        /// <returns>Un IResult</returns>
        Task<IResult> UploadAsync(ProfilePictureUpload request);
    }
}
