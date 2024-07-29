using IGift.Application.Interfaces.Files;
using IGift.Application.Requests.Files.ProfilePicture;
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

        public async Task<IResult> SaveProfilePictureAsync(ProfilePictureUpload request)
        {
            var response = await _uploadService.UploadAsync(request.UploadRequest, true);
            if (!string.IsNullOrEmpty(response))
            {
                var result = await _dbContext.ProfilePicture.Where(x => x.IdUser == request.IdUser).FirstAsync();
                result.Url = response;
                return await Result.SuccessAsync();
            }
            return await Result.FailAsync();
        }
    }
}