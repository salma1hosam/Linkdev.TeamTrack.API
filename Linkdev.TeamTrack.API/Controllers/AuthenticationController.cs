using Linkdev.TeamTrack.Contract.DTOs.AuthDtos;
using Linkdev.TeamTrack.Contract.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Linkdev.TeamTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService _authenticationService) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var result = await _authenticationService.RegisterAsync(registerDto);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var result = await _authenticationService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("LogOut")]
        public async Task<ActionResult> LogOut()
        {
            await _authenticationService.LogOutAsync();
            return Ok();
        }
    }
}
