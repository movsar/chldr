﻿using chldr_data.Dto;
using chldr_data.Enums;
using chldr_data.Interfaces;

namespace chldr_data.Models
{
    public class ActiveSession
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public SessionStatus Status { get; set; } = SessionStatus.Offline;
        public DateTimeOffset AccessTokenExpiresIn { get; set; }
        public UserDto? User { get; set; } = null;

        public void Clear()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
            Status = SessionStatus.Anonymous;
            AccessTokenExpiresIn = DateTimeOffset.MinValue;
            User = null;
        }
    }
}