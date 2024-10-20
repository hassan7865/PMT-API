
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServices.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByEmail(string Email);
    }
}
