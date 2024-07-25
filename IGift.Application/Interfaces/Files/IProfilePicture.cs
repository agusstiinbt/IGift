﻿using IGift.Application.Requests.Files;
using IGift.Application.Responses.Files;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Files
{
    public interface IProfilePicture
    {
        Task<IResult<ProfilePictureResponse>> GetByUserIdAsync(string IdUser);
        Task<IResult> UploadProfileAsync(UploadRequest request);
    }
}
