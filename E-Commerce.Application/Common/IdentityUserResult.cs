using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public class IdentityUserResult
    {
        public IdentityUserResult(string Id , string UserName, string DisplayName, string Email) 
        { 
            this.Id = Id;
            this.UserName = UserName;
            this.DisplayName = DisplayName;
            this.Email = Email;
        }
        public string Id { get; set; } = default!;
        public string UserName { get; set; } =  default!;
        public string DisplayName { get; set; } = default!;
        public string Email { get; set; } = default!;


    }
}
