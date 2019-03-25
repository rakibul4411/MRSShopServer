using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MRSShop.AppIdentity.Data;
using MRSShop.AppIdentity.Models;
using MRSShop.AppIdentity.ViewModel;

namespace MRSShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private IdentityDataContext _context;
        private RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(IdentityDataContext context, UserManager<AppUser> userManager, IConfiguration configuration, RoleManager<AppRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }
        AuthController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        //api/Auth/login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userId = user.Id;
                var users = _userManager.Users.Where(i => i.Id == userId);

                var userWithRole = _context.UserRoles.ToList();

                var roles = _roleManager.Roles.ToList();


                var result = from u in users
                             join ur in userWithRole
                             on u.Id equals ur.UserId
                             join r in roles
                             on ur.RoleId equals r.Id
                             where u.Id == userId
                             select r.Name;

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Typ, result.SingleOrDefault().ToString())
                };

                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecureKey"));


                var token = new JwtSecurityToken(
                    issuer: "api/Auth/login",
                    audience: "api/Auth/login",
                    expires: DateTime.UtcNow.AddMinutes(30),
                    claims: claims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)

                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });

            }

            return Unauthorized();

        }

        //api/Auth/register
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody]RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Street = model.Street,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Country = model.Country                
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, "User");

                    return Ok();
                }

            }

            // If we got this far, something failed, redisplay form
            return NoContent();
        }

    }
}