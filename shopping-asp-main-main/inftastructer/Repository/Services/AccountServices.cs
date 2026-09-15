using AutoMapper;
using core.Dto;
using core.Entities;
using core.interfaces;
using core.Services;
using core.shareing;
using inftastructer.Data.Configration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Repository.Services
{
    public  class AccountServices: IAccountServic
    {
      
        private readonly IEmailServices _emailService;
        private readonly IUnitOfWork work;
        private readonly IConfiguration configuration;



        public AccountServices(IEmailServices emailService, IUnitOfWork work, IConfiguration configuration)
        {

            _emailService = emailService;
            this.work = work;
            this.configuration = configuration;
        }

        public async Task<(bool Success, string Message)> ActiveAccount(ActiveAccountDto activeAccount)
        {
            if (string.IsNullOrWhiteSpace(activeAccount.email) || string.IsNullOrWhiteSpace(activeAccount.code))
                return (false, "Email or code cannot be empty.");

            return await work.AuthRepository.ActiveAccountAsync(activeAccount);
        }

        public async Task<(bool Success, string Message)> ActiveAccountAsync(ActiveAccountDto activeAccount)
        {
            return await work.AuthRepository.ActiveAccountAsync(activeAccount);
        }

        public async Task<string> changepassword(ChangePasswordDto changepassword)
        {
            if (string.IsNullOrWhiteSpace(changepassword.email) ||
           string.IsNullOrWhiteSpace(changepassword.OldPassword) ||
           string.IsNullOrWhiteSpace(changepassword.newpassword))
                return "Invalid data";

            if (changepassword.OldPassword == changepassword.newpassword)
                return "New password must be different";

            return  await work.AuthRepository.changepassword(changepassword);
        }

        public async Task<(bool Success, string Code, string Message)> ForgetPasswordAsync(string email)
        {
           return await work.AuthRepository.ForgetPasswordAsync(email);
        }

        public async Task<string> LoginAsync(loginDto loginDto)
        {
         return await work.AuthRepository.LoginAsync(loginDto);
        }

        public async Task<string> RegisterAsync(registerDto registerDto)
        {
           return await work.AuthRepository.RegisterAsync(registerDto);
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string code, string newPassword)
        {
            return await work.AuthRepository.ResetPasswordAsync(email, code, newPassword);
        }

        public async Task SendVerificationCode(AppUser user)
        {
            var userId = user.Id;
            var email = user.Email;

            string baseUrl = configuration["EmailSetting:BaseUrl"];

            // توليد كود عشوائي
            string code = new Random().Next(100000, 999999).ToString();

            string component = "Verify Account";
            string subject = "Verify Your Account";
            string message = "Please click the button below to verify your account.";

            string emailContent = Emailstringbody.SendEmail(
                email,
                userId,
                baseUrl,
                code,
                component,
                subject,
                message
            );

            // حفظ الكود
            user.VerificationCode = code;
            user.CodeExpiry = DateTime.UtcNow.AddMinutes(10);

            await work.accountRepository.UpdateAsync(user);

            await _emailService.SendEmailAsync(new EmailDto
            {
                to = email,
                subject = subject,
                content = emailContent
            });
        }


        public async Task<bool> VerifyCode(string email, string code)
        {
            var user = await work.accountRepository.GetByEmailAsync(email);
            if (user == null) return false;

            if (user.VerificationCode != code) return false;
            if (user.CodeExpiry < DateTime.UtcNow) return false;

            user.IsEmailConfirmed = true;
            user.VerificationCode = null;
            user.CodeExpiry = null;

            await work.accountRepository.UpdateAsync(user);
            return true;
        }

     

       



    }
}
