using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface ITokenServices
    {
        string CreateToken(string userId, string userName, string email, IEnumerable<string> roles);
    }
}
