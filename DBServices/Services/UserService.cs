
using DBServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServices.Services
{
   
    public class UserService : IUserService
    {
        private readonly PMTContext _context;
       
        public UserService(PMTContext pMTContext)
        {
            _context = pMTContext;
        }

        public async Task<User> GetUserByEmail(string Email)
        {
           var user =  await _context.Users.FirstOrDefaultAsync(e=>e.Email == Email);
            return user;
        }
    }
}
