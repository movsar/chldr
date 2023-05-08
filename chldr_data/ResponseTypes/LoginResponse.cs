﻿using chldr_data.Dto;

namespace chldr_data.ResponseTypes
{
    public class LoginResponse : MutationResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTimeOffset? AccessTokenExpiresIn { get; set; }
        public UserDto? User { get; set; }
    }
}