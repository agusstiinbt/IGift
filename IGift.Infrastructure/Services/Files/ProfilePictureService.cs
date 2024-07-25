using IGift.Application.Interfaces.Files;
using IGift.Application.Responses.Files;
using IGift.Infrastructure.Data;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace IGift.Infrastructure.Services.Files
{
    public class ProfilePictureService : IProfilePicture
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public ProfilePictureService(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        public async Task<Result<ProfilePictureResponse>> GetByUserIdAsync(string IdUser)
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
    }
}
