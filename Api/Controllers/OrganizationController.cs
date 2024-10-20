using Api.AppHelper;
using DBServices.DTOS;
using DBServices.Interfaces;
using DBServices.Services;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : Controller
    {
        private readonly GenericService<User> _genericUser;
        private readonly GenericService<Organization> _genericorganization;
        private readonly IUserService _userService;
        private readonly CommonModule _commonModule;
        public OrganizationController(GenericService<User> genericServiceUser,GenericService<Organization> genericServiceOrg,IUserService userService,CommonModule commonModule)
        {
            _genericorganization = genericServiceOrg;
            _genericUser = genericServiceUser;
            _userService = userService;
            _commonModule = commonModule;
        }


        [HttpPost("Save")]
        public async Task<IActionResult> AddOrganization(OrganizationUser organizationUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid Model", status = false, modelState = ModelState });
                }
                var User = organizationUser.User;
                var Org = organizationUser.Organization;
                var ExUser = await _userService.GetUserByEmail(User.Email);
                if (ExUser != null && ExUser.UserId != User.UserId)
                {
                    return Conflict(new { message = "Email Already Exist", status = false });
                }
                if(Org.OrgId == 0)
                {
                    
                    await _genericorganization.AddAsync(Org);
                    User.OrgId = Org.OrgId;
                    var Password = _commonModule.Encrypt(User.Password);
                    User.Password = Password;
                    User.RoleId = 1;

                    await _genericUser.AddAsync(User);
                    
                }
                else if(Org.OrgId > 0)
                {
                    var exDbOrg = await _genericorganization.GetByIdAsync(Org.OrgId);
                    if (exDbOrg == null)
                    {
                        return NotFound(new { message = "Organization Not Found", status = false });

                    }
                    await _genericorganization.UpdateAsync(exDbOrg, Org);
                }

                return Ok(new { message = "Saved Successfully", status = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server Error", innerException = ex.InnerException });
            }
        }
    }
}
