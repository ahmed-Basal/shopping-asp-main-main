using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace core.Entities
{
    public  class AppUser:IdentityUser
    {
     
        public string DisplayName { get; set; }
        public Address ? Address { get; set; }
        public string? VerificationCode { get; set; }
        public DateTime? CodeExpiry { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsFirstLogin { get; set; } = true;
        public ICollection<comment> Comments { get; set; } = new List<comment>();
    }
}
