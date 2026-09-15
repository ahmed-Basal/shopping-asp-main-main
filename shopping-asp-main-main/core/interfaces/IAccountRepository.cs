using core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.interfaces
{
    public  interface  IAccountRepository
    {
        Task<AppUser> GetByEmailAsync(string email);
        Task<AppUser> GetByIdAsync(string id);
        Task UpdateAsync(AppUser user);
    }
}
