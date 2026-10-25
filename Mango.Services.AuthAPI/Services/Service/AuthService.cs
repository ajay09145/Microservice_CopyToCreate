using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db,UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == email.ToLower());
            if (user != null) {
                if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                //else
                //{
                //    return Task.FromResult(false);
                //}
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResonseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user =  _db.ApplicationUsers.FirstOrDefault(u =>  u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || isValid == false)
            {
                return new LoginResonseDto() { User = null, Token = "" };
            }

            //if token was found, generate jwt token
            var roles = await _userManager.GetRolesAsync(user);  
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDto = new()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };
            LoginResonseDto loginResonseDto = new LoginResonseDto()
            {
                User = userDto,
                Token = token           //Pass here token
            };
            return loginResonseDto;
        }

        public async Task<string> RegisterAsync(RegistrationRequestDto registerationRequestDto)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registerationRequestDto.Email,
                Email = registerationRequestDto.Email,
                Name = registerationRequestDto.Name,
                PhoneNumber = registerationRequestDto.PhoneNumber,
                NormalizedEmail = registerationRequestDto.Email.ToUpper(),
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDto.Password);
                if(result.Succeeded)
                {
                    //if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                    //{
                    //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    //    await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    //}
                    //await _userManager.AddToRoleAsync(user, "Admin");
                    var userToReturn =  _db.ApplicationUsers.First(u=> u.UserName == registerationRequestDto.Email);
                    var userDto = new UserDto()
                    {
                        Id = userToReturn.Id,
                        Name = userToReturn.Name,
                        Email = userToReturn.Email,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "userDto";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }  
            }
            catch (Exception ex)
            {

                throw;
            }
            return "Error Encounter";
        }
    }
}
