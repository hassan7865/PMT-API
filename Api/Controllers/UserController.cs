using Api.AppHelper;
using DBServices.DTOS;
using DBServices.Interfaces;
using DBServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private GenericService<User> _userGenericService;
        private IUserService _userService;
        private CommonModule _commonModule;
        private JwtDTO _jwtDTO;
       
        public UserController(GenericService<User> genericService, IUserService userService, CommonModule commonModule,IConfiguration configuration)
        {

            _userGenericService = genericService;
            _userService = userService;
            _commonModule = commonModule;
            _jwtDTO = configuration.GetSection("Jwt").Get<JwtDTO>();
        }

        

        [HttpPost("Save")]
        [Authorize]


        public async Task<IActionResult> SaveUser(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid Model", status = false, modelState = ModelState });
                }

                var ExUser = await _userService.GetUserByEmail(user.Email);
                if (ExUser != null && ExUser.UserId != user.UserId)
                {
                    return Conflict(new { message = "Email Already Exist", status = false });
                }
                if (user.UserId == 0)
                {

                    var Pass = _commonModule.Encrypt(user.Password);
                    user.OrgId = _commonModule.GetUserOrg();
                    user.Password = Pass;
                    

                    await _userGenericService.AddAsync(user);
                }
                else if (user.UserId > 0)
                {
                    var exDBUser = await _userGenericService.GetByIdAsync(user.UserId);
                    if (exDBUser == null)
                    {
                        return NotFound(new { message = "User Not Found", status = false });

                    }
                    user.UpdatedAt = DateTime.Now;

                    await _userGenericService.UpdateAsync(exDBUser, user);
                }

                return Ok(new { message = "Saved Successfully", status = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server Error", innerException = ex.InnerException });
            }
        }

        [HttpPost("Login")]

        public async Task<IActionResult> UserLogin([FromBody] UserLogin userData)
        {
            try
            {
                var user = await _userService.GetUserByEmail(userData.Email);
                if (user == null)
                {
                    return NotFound(new { message = "Invalid Credentials", status = false });
                }
                var Dbpass = _commonModule.Decrypt(user.Password);
                if (userData.Password != Dbpass)
                {
                    return NotFound(new { message = "Invalid Credentials", status = false });
                }
                var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtDTO.Key));
                var cred = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                        {
                          new Claim("UserId", user.UserId.ToString()),
                          new Claim("RoleId", user.RoleId.ToString()),
                          new Claim("OrgId",user.OrgId.ToString())
                        };

                var sToken = new JwtSecurityToken(_jwtDTO.Key, _jwtDTO.Issuer, claims, expires: DateTime.Now.AddHours(5), signingCredentials: cred);
                var token = new JwtSecurityTokenHandler().WriteToken(sToken);
                var userAuth = new
                {
                    UserId = user.UserId,
                    RoleId = user.RoleId,
                    Token = token
                };
              

                return Ok(new {userAuth,message = "Login Successfully"});

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server Error", innerException = ex.InnerException });
            }


        }
    }
}