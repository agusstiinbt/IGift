﻿using System.Net.Http.Json;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Identity.Users;
using IGift.Application.Responses.Identity.Users;
using IGift.Shared.Wrapper;
using Microsoft.JSInterop;

namespace Client.Infrastructure.Services.Identity.Users
{
    public class UserManager : IUserManager
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public UserManager(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task<IResult<UserResponse>> GetUserById(string UserId)
        {
            var request = new UserByIdRequest { UserId = UserId };
            var response = await _http.PostAsJsonAsync(IGift.Shared.Constants.AppConstants.Controller.Users.GetById, request);
            return await response.ToResult<UserResponse>();
        }

        public async Task<IResult<List<UserResponse>>> GetUsersAsync()
        {
            //TODO el applicationUSerResponse debe cargar la lista de giftcards ordenadas primero por activas y fecha de creación
            var response = await _http.GetAsync(IGift.Shared.Constants.AppConstants.Controller.Users.GetAll);
            return await response.ToResult<List<UserResponse>>();
        }
    }
}
