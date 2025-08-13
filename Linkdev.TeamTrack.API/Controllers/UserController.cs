using Linkdev.TeamTrack.Contract.DTOs.UserDtos;
using Linkdev.TeamTrack.Contract.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Linkdev.TeamTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService _userService) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _userService.RegisterAsync(registerDto);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            var result = await _userService.LogOutAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AssignUserRole")]
        public async Task<IActionResult> AssignUserRole(SetUserRoleDto setUserRoleDto)
        {
            var result = await _userService.AssignUserRoleAsync(setUserRoleDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateUserRole")]
        public async Task<IActionResult> UpdateUserRole(SetUserRoleDto setUserRoleDto)
        {
            var result = await _userService.UpdateUserRoleAsync(setUserRoleDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(UserFilterParams userQueryParams)
        {
            var result = await _userService.GetAllUsersAsync(userQueryParams);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllProjectManagers")]
        public async Task<IActionResult> GetAllProjectManagers()
        {
            var result = await _userService.GetAllProjectManagersAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpGet("GetAllTeamMembers")]
        public async Task<IActionResult> GetAllTeamMembers()
        {
            var result = await _userService.GetAllTeamMembersAsync();
            return Ok(result);
        }
    }
}
