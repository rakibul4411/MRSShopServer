using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MRSShop.AppIdentity.Data;
using MRSShop.AppIdentity.Models;
using MRSShop.AppIdentity.ViewModel;

namespace MapServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IdentityDataContext _context;
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        public UserController(IdentityDataContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //api/User
        [HttpGet]
        public IActionResult Get()
        {
            var user = _userManager.Users.ToList();
            return Ok(user);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string Id)
        {
            AppUser user = await _userManager.FindByIdAsync(Id);
            UserVM model = new UserVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Street = user.Street,
                City = user.City,
                PostalCode = user.PostalCode,
                Country = user.Country,
                /*Roles = (from r in _roleManager.Roles
                         join ur in _context.UserRoles
                         on r.Id equals ur.RoleId
                         join u in _userManager.Users
                         on ur.UserId equals u.Id
                         where u.Id == Id
                         select r.Name).ToList()*/
                Roles = await _userManager.GetRolesAsync(user)
            };
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]UserVM model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new AppUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, PhoneNumber = model.PhoneNumber, City = model.City, Street = model.Street, Country = model.Country };
                var user = _userManager.CreateAsync(newUser, model.Password).Result;
                if (user.Succeeded)
                {
                    await _userManager.AddToRolesAsync(newUser, model.Roles);
                    return Ok();
                }

                return Ok();
            }


            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromBody] UserVM updateUser)
        {
            var model = await _userManager.FindByIdAsync(updateUser.Id);
            if (model != null)
            {


                if (User.IsInRole("admin") == true)
                {
                    await _userManager.AddToRolesAsync(model, updateUser.Roles);

                }
                else
                {
                    model.UserName = updateUser.Email;
                    model.PhoneNumber = updateUser.PhoneNumber;
                    model.Email = updateUser.Email;
                    model.FirstName = updateUser.FirstName;
                    model.LastName = updateUser.LastName;
                    model.PasswordHash = updateUser.Password;
                    model.Street = updateUser.Street;
                    model.City = updateUser.City;
                    model.PostalCode = updateUser.PostalCode;
                    model.Country = updateUser.Country;
                }


                var Update = await _userManager.UpdateAsync(model);

                return Ok(updateUser);
            }

            return NoContent();
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            if (Id != null)
            {
                var user = await _userManager.FindByIdAsync(Id);
                var delete = await _userManager.DeleteAsync(user);
                return Ok();
            }
            return Ok();
        }
    }
}