using System;
using System.Collections.Generic;
using System.Text;

namespace core.Services
{
    public  interface  IEmailServices
    {
        Task SendEmailAsync(core.Dto.EmailDto emailDto);
    }
}
