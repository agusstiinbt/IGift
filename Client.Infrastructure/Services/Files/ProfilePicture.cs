using Client.Infrastructure.Extensions;
using IGift.Application.Requests.Files;
using IGift.Application.Responses.Files;
using IGift.Shared;
using IGift.Shared.Wrapper;
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
            var request = new ProfilePictureRequest { Id = Id };
            var response = await _httpClient.PostAsJsonAsync(AppConstants.Controllers.FilesController.GetProfilePictureById, request);
            return await response.ToResult<ProfilePictureResponse>();
        }
    }
}
