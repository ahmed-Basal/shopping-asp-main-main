using AutoMapper;
using Azure;
using core.Dto;
using core.Entities;
using core.interfaces;
using core.Services;
using core.shareing;
using inftastructer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Crypto.Macs;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Repository.Services
{
    public class AuthRepository : IAuth
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<core.Entities.AppUser> _userManager;
        private readonly IEmailServices _emailSender;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<core.Entities.AppUser> _signInManager;
        private readonly ITokenGenerate tokenGenerate;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        public IEmailServices EmailServices { get; }

        public AuthRepository(UserManager<AppUser> userManager, IEmailServices emailSender, SignInManager<AppUser> signInManager, ITokenGenerate tokenGenerate, IConfiguration configuration, AppDbContext dbContext)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            this.tokenGenerate = tokenGenerate;
            _configuration = configuration;
            _dbContext = dbContext;
        }


        private async Task<string> GenerateSixDigitCode(AppUser user)
        {
            Random rnd = new Random();
            string code = rnd.Next(100000, 1000000).ToString();

            // نفترض إن AppUser عنده خصائص VerificationCode و CodeExpiry
            user.VerificationCode = code;
            user.CodeExpiry = DateTime.UtcNow.AddMinutes(10); // صلاحية الكود 10 دقايق
            await _userManager.UpdateAsync(user);

            return code;
        }

        //  public async Task<string?> RegisterAsync(registerDto dto)
        //  {
        //      if (dto == null)
        //          return "Invalid data";

        //      // check email
        //      var existingEmail = await _userManager.FindByEmailAsync(dto.email);
        //      if (existingEmail != null)
        //          return "This email is already registered";

        //      // check username
        //      var existingUsername = await _userManager.FindByNameAsync(dto.UserName);
        //      if (existingUsername != null)
        //          return "This username already exists";

        //      // create user
        //      var user = new AppUser
        //      {
        //          DisplayName = dto.Displayname,
        //          Email = dto.email,
        //          UserName = dto.UserName,
        //          EmailConfirmed = false
        //      };

        //      var createResult = await _userManager.CreateAsync(user, dto.Password);

        //      if (!createResult.Succeeded)
        //          return createResult.Errors.FirstOrDefault()?.Description;

        //      // generate 6-digit confirmation code
        //      string code = await GenerateSixDigitCode(user); // ده الكود الجديد

        //      // send email with 6-digit code
        //      await SendEmail(
        //          user.Email,
        //          user.Id,
        //          code, // نرسل الكود بدل الـ token
        //          "Activate Account",
        //          "Confirm Email",
        //          $"Your confirmation code is: {code}"
        //      );

        //      return null;
        //  }


        //  public async Task SendEmail(
        //string email,
        //string userId,
        //string code,
        //string component,
        //string subject,
        //string message)
        //  {
        //      string baseUrl = _configuration["EmailSetting:BaseUrl"];

        //      var body = Emailstringbody.SendEmail(
        //          email,
        //          userId,
        //          baseUrl,
        //          code,
        //          component,
        //          subject,
        //          message
        //      );

        //      var result = new EmailDto(
        //          email,
        //          "noreply@yourapp.com",
        //          subject,
        //          body
        //      );

        //      await _emailSender.SendEmailAsync(result);
        ////  
        public async Task<string> RegisterAsync(registerDto register)
        {
            if (register == null)
                return "Invalid data";

            var existingEmail = await _userManager.FindByEmailAsync(register.email);
            if (existingEmail != null)
                return "This email is already registered";

            var existingUsername = await _userManager.FindByNameAsync(register.UserName);
            if (existingUsername != null)
                return "This username already exists";

            var user = new AppUser
            {
                DisplayName = register.Displayname,
                Email = register.email,
                UserName = register.UserName,
                EmailConfirmed = false,
                IsFirstLogin = true
            };

            var result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
                return result.Errors.FirstOrDefault()?.Description ?? "Registration failed";

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://localhost:5239/activate?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            return null!;
        }

        public async Task<string> LoginAsync(loginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.email);
            if (user == null)
                return "Invalid email or password.";
            if (user != null && user.IsFirstLogin)
            {
              
                // تحديث العلم عشان مايتعادش الإرسال
                user.IsFirstLogin = false;
                await _userManager.UpdateAsync(user);
            }
            if (!user.EmailConfirmed)
            {
                // إرسال تأكيد البريد...
                return "Please confirm your email address. A confirmation email has been sent to your inbox.";
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
            if (!result.Succeeded)
                return "Invalid email or password.";

            // توليد Token فقط
            return tokenGenerate.GetAndCreateToken(user);
        }

        public async Task<AuthResponseDto> LoginWithRefreshTokenAsync(loginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.email);
            if (user == null)
                return new AuthResponseDto { IsAuthenticated = false, Message = "Invalid email or password." };

            if (user.IsFirstLogin)
            {
                user.IsFirstLogin = false;
                await _userManager.UpdateAsync(user);
            }

            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto { IsAuthenticated = false, Message = "Please confirm your email address. A confirmation email has been sent to your inbox." };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
            if (!result.Succeeded)
                return new AuthResponseDto { IsAuthenticated = false, Message = "Invalid email or password." };

            var jwtToken = tokenGenerate.GetAndCreateToken(user);
            var refreshToken = tokenGenerate.GenerateRefreshToken();
            refreshToken.AppUserId = user.Id;

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Message = "Login successful",
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return new AuthResponseDto { IsAuthenticated = false, Message = "Token is required." };

            var refreshToken = await _dbContext.RefreshTokens
                .Include(r => r.AppUser)
                .SingleOrDefaultAsync(r => r.Token == token);

            if (refreshToken == null)
                return new AuthResponseDto { IsAuthenticated = false, Message = "Invalid refresh token." };

            if (!refreshToken.IsActive)
                return new AuthResponseDto { IsAuthenticated = false, Message = "Refresh token is inactive or expired." };

            // Token Rotation: Revoke current token
            refreshToken.RevokedOn = DateTime.UtcNow;

            var user = refreshToken.AppUser;
            if (user == null)
            {
                user = await _userManager.FindByIdAsync(refreshToken.AppUserId);
                if (user == null)
                    return new AuthResponseDto { IsAuthenticated = false, Message = "User not found." };
            }

            var newJwtToken = tokenGenerate.GetAndCreateToken(user);
            var newRefreshToken = tokenGenerate.GenerateRefreshToken();
            newRefreshToken.AppUserId = user.Id;

            await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Message = "Token refreshed successfully.",
                Token = newJwtToken,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn
            };
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var refreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(r => r.Token == token);
            if (refreshToken == null || !refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<string> changepassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(changePasswordDto.email);
            if (user == null)
                return "User not found";

            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordDto.OldPassword,
                changePasswordDto.newpassword
            );

            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            return "Password changed successfully";
        }


        public async Task<(bool Success, string Message)> ActiveAccountAsync(ActiveAccountDto activeAccount)
        {
            // جلب المستخدم بالكامل
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == activeAccount.email);

            if (user == null)
                return (false, "User not found.");

            if (user.EmailConfirmed)
                return (true, "Account already confirmed.");

            if (string.IsNullOrEmpty(user.VerificationCode) ||
                user.VerificationCode != activeAccount.code ||
                user.CodeExpiry == null ||
                user.CodeExpiry < DateTime.UtcNow)
            {
                return (false, "Invalid or expired code.");
            }

            // تحديث حالة الحساب للتأكيد
            user.EmailConfirmed = true;
            user.VerificationCode = null;
            user.CodeExpiry = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            // التأكد من أن التغيير حفظ في قاعدة البيانات
            var checkUser = await _userManager.FindByEmailAsync(activeAccount.email);
            if (!checkUser.EmailConfirmed)
                return (false, "Failed to confirm email in database.");

            return (true, "Account activated successfully.");
        }

        public async Task<(bool Success, string Code, string Message)> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, null!, "User not found.");

            // توليد كود 6 أرقام
            string code = await GenerateSixDigitCode(user);

            // إرسال الكود على البريد
            await _emailSender.SendEmailAsync(new EmailDto
            {
                to = user.Email,
                from = _configuration["EmailSetting:form"],
                subject = "Reset Your Password",
                content = $"<p>Your password reset code is: <strong>{code}</strong></p>"
            });

            return (true, code, "Password reset code sent to your email.");
        }

        // 2️⃣ إعادة تعيين كلمة المرور باستخدام الكود
        public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string code, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "User not found.");

            if (string.IsNullOrEmpty(user.VerificationCode) || user.VerificationCode != code || user.CodeExpiry < DateTime.UtcNow)
                return (false, "Invalid or expired code.");

            // توليد Token لإعادة تعيين كلمة المرور
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            // تنظيف الكود وصلاحيته بعد إعادة التعيين
            user.VerificationCode = null;
            user.CodeExpiry = null;
            await _userManager.UpdateAsync(user);

            return (true, "Password reset successfully.");
        }

        public async Task<bool> updateaddress(string email, Address address)
        {
           var finduser= _userManager.Users.FirstOrDefault(u => u.Email == email);
            if (finduser == null)
                return false;
           var myaddress=await _dbContext.address.FirstOrDefaultAsync(a => a.AppuserId == finduser.Id);
            if (myaddress is  null)
            {
               await _dbContext.address.AddAsync(myaddress);
            }
            else
            {
                address.AppuserId = finduser.Id;
                 _dbContext.address.Update(address);
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}