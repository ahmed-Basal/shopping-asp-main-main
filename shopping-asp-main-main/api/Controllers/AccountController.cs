using api.halper;
using AutoMapper;
using core.Dto;
using core.Entities;
using core.interfaces;
using core.Services;
using inftastructer.Repository;
using inftastructer.Repository.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Security.Claims;

namespace api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServic _account;
        private readonly IUnitOfWork _unitOfWork;
      private readonly IMapper _mapper;
        public AccountController(IAccountServic account, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _account = account;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpPut("update-address")]
        public async Task<IActionResult> UpdateAddress([FromBody] ShipaddressDto addressDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return Unauthorized();

            var address = _mapper.Map<Address>(addressDto);

            var result = await _unitOfWork.AuthRepository.updateaddress(email, address);
               
            if (!result)
                return BadRequest("Update failed");

            return Ok("Updated successfully");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(registerDto dto)
        {
            var result = await _account.RegisterAsync(dto);

            if (result != null)
                return BadRequest(new { message = result });

            return Ok(new { message = "Account created successfully. Please check your email." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(loginDto dto)
        {
            var response = await _account.LoginWithRefreshTokenAsync(dto);

            if (!response.IsAuthenticated)
                return BadRequest(new { message = response.Message });

            SetAuthCookies(response.Token!, response.RefreshToken!, response.RefreshTokenExpiration);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto? dto)
        {
            var token = Request.Cookies["refreshToken"] ?? dto?.Token;
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Refresh token is required." });

            var response = await _account.RefreshTokenAsync(token);
            if (!response.IsAuthenticated)
                return BadRequest(new { message = response.Message });

            SetAuthCookies(response.Token!, response.RefreshToken!, response.RefreshTokenExpiration);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto? dto)
        {
            var token = Request.Cookies["refreshToken"] ?? dto?.Token;
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required." });

            var result = await _account.RevokeTokenAsync(token);
            if (!result)
                return BadRequest(new { message = "Token is invalid or already revoked." });

            Response.Cookies.Delete("authToken");
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Token revoked successfully." });
        }

        private void SetAuthCookies(string accessToken, string refreshToken, DateTime? refreshTokenExpiration)
        {
            var isHttps = Request.IsHttps;

            Response.Cookies.Append("authToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = SameSiteMode.Lax,
                Expires = refreshTokenExpiration ?? DateTime.UtcNow.AddDays(7)
            });
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var result = await _account.changepassword(dto);

            if (result == "Password changed successfully")
                return Ok(result);

            return BadRequest(result);
        }


        [HttpGet("ActiveEmail")]
        public async Task<IActionResult> ActiveEmail([FromQuery] ActiveAccountDto dto)
        {
           
            var (success, message) = await _account.ActiveAccountAsync(dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromForm] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required.");

            var result = await _account.ForgetPasswordAsync(email);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message, code = result.Code });
        }



            [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromForm] string email,
        [FromForm] string code,
        [FromForm] string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(newPassword))
                return BadRequest("Email, code, and new password are required.");

            var result = await _account.ResetPasswordAsync(email, code, newPassword);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
      
            
        }

    }

}

