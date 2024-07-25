using Client.Infrastructure.Extensions;
using IGift.Application.Requests.Files.ProfilePicture;
using IGift.Application.Responses.Files;
using IGift.Shared;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

namespace IGift.Client.Infrastructure.Services.Files
{
    public class ProfilePictureService : IProfilePicture
    {
        private readonly HttpClient _httpClient;

        public ProfilePictureService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<ProfilePictureResponse>> GetByIdAsync(string Id)
        {
            var request = new ProfilePictureById { Id = Id };
            var response = await _httpClient.PostAsJsonAsync(AppConstants.Controllers.FilesController.GetProfilePictureById, request);
            return await response.ToResult<ProfilePictureResponse>();
        }

        public async Task<IResult> UploadAsync(IBrowserFile e)
        {
            var request = new ProfilePictureUpload();
            request.File = e;
            var response = await _httpClient.PostAsJsonAsync(AppConstants.Controllers.FilesController.UploadProfilePicture, request);
            return await response.ToResult();
        }
    }
}
