using IGift.Application.CQRS.Files.ProfilePicture;
using IGift.Application.Enums;
using IGift.Application.Interfaces.Files;
using IGift.Application.Responses.Files;
using IGift.Infrastructure.Data;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IGift.Infrastructure.Services.Files
{
    public class ProfilePictureService : IProfilePicture
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;//Esta propiedad sabe de donde se esta haciendo el llamado, por eso sabe donde buscar la imagen
        private readonly ILogger<ProfilePictureService> logger;
        private readonly IUploadFileService _uploadService;

        public ProfilePictureService(ApplicationDbContext dbContext, IWebHostEnvironment env, ILogger<ProfilePictureService> logger, IUploadFileService uploadService)
        {
            _dbContext = dbContext;
            _env = env;
            this.logger = logger;
            _uploadService = uploadService;
        }

        /// <summary>
        /// Devuelve la informacion de foto de perfil del IdUser
        /// </summary>
        /// <param name="IdUser"></param>
        /// <returns></returns>
        public async Task<IResult<ProfilePictureResponse>> GetByUserIdAsync(string IdUser)
        {
            try
            {
                var result = await _dbContext.ProfilePicture.Where(x => x.IdUser == IdUser).ToListAsync();

                if (result.Any())
                {

                    var response = result.FirstOrDefault();
                    if (string.IsNullOrEmpty(response.Url))
                    {
                        return await Result<ProfilePictureResponse>.FailAsync("No se ha encontrado la foto de perfil");
                    }

                    var filePath = Path.Combine(_env.ContentRootPath, response.Url!);
                    if (!File.Exists(filePath))
                    {
                        return await Result<ProfilePictureResponse>.FailAsync("No se ha encontrado la foto de perfil");
                    }

                    var data = await File.ReadAllBytesAsync(filePath);
                    var fileType = response.FileType;

                    var profilePicture = new ProfilePictureResponse { Data = data, FileType = fileType, UploadDate = response.UploadDate };

                    return Result<ProfilePictureResponse>.Success(profilePicture);
                }

            }
            catch (Exception e)
            {

                throw;
            }
            return await Result<ProfilePictureResponse>.FailAsync();
        }

        /// <summary>
        /// Devuelve la informacion de foto de perfil del IdUser
        /// </summary>
        /// <param name="IdUser"></param>
        /// <returns></returns>
        public async Task<ProfilePictureResponse> GetByUserIdAsync2(string IdUser)
        {
            var response = await _dbContext.ProfilePicture.Where(x => x.IdUser == IdUser).ToListAsync();

            if (response.Any())
            {
                var result = response.FirstOrDefault();

                if (string.IsNullOrEmpty(result.Url))
                    return null;

                var filePath = Path.Combine(_env.ContentRootPath, result.Url!);
                if (!File.Exists(filePath))
                    return null;

                var data = await File.ReadAllBytesAsync(filePath);
                var fileType = result.FileType;

                var profilePicture = new ProfilePictureResponse { Data = data, FileType = fileType, UploadDate = result.UploadDate };

                return profilePicture;
            }
            return null;

        }


        /// <summary>
        /// Guarda una foto de perfil en una carpeta del servidor y en el caso de que sea de un nuevo usuario se crea la informacion en la bbdd
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IResult> SaveProfilePictureAsync(ProfilePictureUpload request)
        {

            if (request.UploadRequest.UploadType != UploadType.ProfilePicture)
                return await Result<IResult>.FailAsync();

            //Las fotos de perfil siempre las pisamos, es decir eliminamos la anterior
            var pathResponse = await _uploadService.UploadAsync(request.UploadRequest, true);

            if (!string.IsNullOrEmpty(pathResponse))
            {
                var newProfilePicture = new ProfilePicture { FileType = "image/png", IdUser = request.IdUser, UploadDate = DateTime.Now, Url = pathResponse };

                var exists = await _dbContext.ProfilePicture.AnyAsync(x => x.IdUser == request.IdUser);
                try
                {
                    if (!exists)
                    {
                        await _dbContext.ProfilePicture.AddAsync(newProfilePicture);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error al subir foto de perfil");
                }
                //Si no existe una foto de perfil con el IdUser del 'request' entonces creamos una nueva fila

                var user = await _dbContext.Users.Where(x => x.Id == request.IdUser).FirstAsync();

                user.ProfilePictureDataUrl = pathResponse;

                await _dbContext.SaveChangesAsync();

                return await Result.SuccessAsync();
            }

            return await Result.FailAsync();
        }
    }
}