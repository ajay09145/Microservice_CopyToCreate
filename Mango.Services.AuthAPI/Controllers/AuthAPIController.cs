using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private ResponseDto _responseDto;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responseDto = new ResponseDto();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.RegisterAsync(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.Result = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.LoginAsync(model);
            if(loginResponse.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "User or password is incorrect";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var sssignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!sssignRoleSuccessful)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error Encounter";
                return BadRequest(_responseDto);
            }
            _responseDto.IsSuccess = true;
            return Ok(_responseDto);
        }



         


    }
}
