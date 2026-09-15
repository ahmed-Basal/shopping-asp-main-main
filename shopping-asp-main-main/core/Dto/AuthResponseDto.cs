using System;

namespace core.Dto
{
    public class AuthResponseDto
    {
        public bool IsAuthenticated { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }

    public class RefreshTokenDto
    {
        public string? Token { get; set; }
    }
}
