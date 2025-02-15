﻿using System.Net.Http.Json;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Files.ProfilePicture;
using IGift.Application.Responses.Files;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;

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
            var response = await _httpClient.PostAsJsonAsync(ConstFilesController.GetProfilePictureById, request);
            return await response.ToResult<ProfilePictureResponse>();
        }

        public async Task<IResult> UploadAsync(ProfilePictureUpload file)
        {
            var response = await _httpClient.PostAsJsonAsync(ConstFilesController.UploadProfilePicture, file);
            return await response.ToResult();
        }
    }
}
