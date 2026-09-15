using core.Dto;
using core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.interfaces
{
    public  interface  IAuth
    {
        Task<string> RegisterAsync(registerDto registerDto);
        Task<string> LoginAsync(loginDto loginDto);
      
        Task<string> changepassword(ChangePasswordDto changepassword);
        Task<(bool Success, string Message)> ActiveAccountAsync(ActiveAccountDto activeAccount);
         Task<(bool Success, string Code, string Message)> ForgetPasswordAsync(string email);
         Task<(bool Success, string Message)> ResetPasswordAsync(string email, string code, string newPassword);
        Task<bool> updateaddress(string email,Address address);


    }
}
