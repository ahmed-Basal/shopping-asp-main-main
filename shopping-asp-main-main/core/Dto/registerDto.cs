using System;
using System.Collections.Generic;
using System.Text;

namespace core.Dto
{
    public record loginDto
    {
      public string email { get; set; }
        public string Password { get; set; }
        
    }
    public  record  registerDto: loginDto
    {
        public string UserName { get; set; }
        public string Displayname { get; set; }
     
       

    }
    public record ChangePasswordDto
    {
        public string email { get; set; }
        public string OldPassword { get; set; }
            public string newpassword { get; set; }
    }
    public record ActiveAccountDto
    {
        public string email { get; set; }
        public string code { get; set; }
    }
  


}
