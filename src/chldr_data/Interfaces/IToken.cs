﻿using chldr_data.Enums;

namespace chldr_data.Interfaces
{
    public interface IToken
    {
        string TokenId { get; set; }
        string UserId { get; set; }
        string Value { get; set; }
        DateTimeOffset? CreatedAt { get; set; }
        DateTimeOffset? ExpiresIn { get; set; }
    }
}
