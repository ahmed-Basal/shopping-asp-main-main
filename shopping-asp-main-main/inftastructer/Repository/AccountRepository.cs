using core.Dto;
using core.Entities;
using core.interfaces;
using core.Services;
using core.shareing;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Repository.Services
{
    
        public class AccountRepository : IAccountRepository
    {

        private readonly UserManager<AppUser> _userManager;

        public AccountRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AppUser> GetByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<AppUser> GetByIdAsync(string id)
            => await _userManager.FindByIdAsync(id);

        public async Task UpdateAsync(AppUser user)
            => await _userManager.UpdateAsync(user);
    }
    }
    

