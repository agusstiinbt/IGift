﻿using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Identity.Users
{
    public interface IUserManager
    {
        Task<IResult> GetUsersAsync();
    }
}
