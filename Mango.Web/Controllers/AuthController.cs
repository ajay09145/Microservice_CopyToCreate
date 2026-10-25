using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }
        [HttpGet]
        public IActionResult Login()
        {
            //LoginRequestDto loginRequestDto = new();
            //return View(loginRequestDto);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            if(ModelState.IsValid)
            {
                ResponseDto result = await _authService.LoginAsync(obj);
                if (result != null && result.IsSuccess)
                {
                    LoginResonseDto loginResonseDto = JsonConvert.DeserializeObject<LoginResonseDto>(Convert.ToString(result.Result));

                    await SignInUser(loginResonseDto);
                    _tokenProvider.SetToken(loginResonseDto.Token);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("CustomError", result.Message);
                    TempData["error"] = result.Message;
                }
            }
            else
            {
                TempData["error"] = "User name and Password have required.";
            }
            return View(obj);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = SD.RoleAdmin, Value=SD.RoleAdmin},
                new SelectListItem{Text = SD.RoleCustomer, Value=SD.RoleCustomer}
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            ResponseDto result = await _authService.RegisterAsync(obj);
            ResponseDto assignRole;
            if(result != null && result.IsSuccess)
            {
                if(string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignAsync(obj);
                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration succeed";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = result.Message;
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = SD.RoleAdmin, Value=SD.RoleAdmin},
                new SelectListItem{Text = SD.RoleCustomer, Value=SD.RoleCustomer}
            };
            return View(obj);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
             _tokenProvider.ClearToken();

            return RedirectToAction(nameof(Login));
        }


        private async Task SignInUser(LoginResonseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }


    }
}
 