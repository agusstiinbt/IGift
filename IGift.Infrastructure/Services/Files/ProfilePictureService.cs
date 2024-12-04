using IGift.Application.CQRS.Files.ProfilePicture;
using IGift.Application.Enums;
using IGift.Application.Interfaces.Files;
using IGift.Application.Models;
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
        private readonly IUploadService _uploadService;

        public ProfilePictureService(ApplicationDbContext dbContext, IWebHostEnvironment env, ILogger<ProfilePictureService> logger, IUploadService uploadService)
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
            var response = await _dbContext.ProfilePicture.Where(x => x.IdUser == IdUser).FirstAsync();
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

        /// <summary>
        /// Guarda una foto de perfil en una carpeta del servidor y en el caso de sea de un nuevo usuario se crea la informacion en la bbdd
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IResult> SaveProfilePictureAsync(ProfilePictureUpload request)
        {
            //Si estamos subiendo una foto de perfil entonces el nombre debe ser el IdUser
            //TODO esto de acá arriba lo estamos haciendo porque si mandamos el archivo desde el cliente con un fileName = al IdUSer entonces el nombre del archivo va a ser muy largo porque Los IdUser son largos. Fijarse si podemos cambiar el tamaño de los Id de los usuarios por un int mejor

            if (request.UploadRequest.UploadType == UploadType.ProfilePicture)
            {
                request.UploadRequest.FileName = request.IdUser;
            }

            //Las fotos de perfil siempre las pisamos, es decir eliminamos la anterior
            var pathResponse = await _uploadService.UploadAsync(request.UploadRequest, true);

            if (!string.IsNullOrEmpty(pathResponse))
            {
                var newProfilePicture = new ProfilePicture { FileType = "image/png", IdUser = request.IdUser, UploadDate = DateTime.Now, Url = pathResponse };

                var exists = await _dbContext.ProfilePicture.AnyAsync(x => x.IdUser == request.IdUser);
                //Si no existe una foto de perfil con el IdUser del 'request' entonces creamos una nueva fila
                if (!exists)
                {
                    await _dbContext.ProfilePicture.AddAsync(newProfilePicture);
                    await _dbContext.SaveChangesAsync();
                }

                var user = await _dbContext.Users.Where(x => x.Id == request.IdUser).FirstAsync();

                user.ProfilePictureDataUrl = pathResponse;

                await _dbContext.SaveChangesAsync();

                return await Result.SuccessAsync();
            }

            return await Result.FailAsync();
        }
    }
}