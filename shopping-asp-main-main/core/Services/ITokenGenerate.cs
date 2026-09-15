using core.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Services
{
    public  interface ITokenGenerate
    {
      string  GetAndCreateToken(AppUser user);
    }
}
