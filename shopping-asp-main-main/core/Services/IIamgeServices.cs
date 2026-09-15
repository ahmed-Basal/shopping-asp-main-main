using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Services
{
    public  interface  IIamgeServices
    {
        Task<List<string>> AddImageAsync(IFormFile files, string src);
        void DeleteImageAsync(string src);
    }
}
