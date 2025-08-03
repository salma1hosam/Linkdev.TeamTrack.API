using Linkdev.TeamTrack.Contract.DTOs.UserDtos;
using Linkdev.TeamTrack.Core.Responses;
using Linkdev.TeamTrack.Contract.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Linkdev.TeamTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService _userService) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<ActionResult<GenericResponse<UserDto>>> Register(RegisterDto registerDto)
        {
            var result = await _userService.RegisterAsync(registerDto);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<GenericResponse<UserDto>>> Login(LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("LogOut")]
        public async Task<ActionResult<GenericResponse<bool>>> LogOut()
        {
            await _userService.LogOutAsync();
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AssignUserRole")]
        public async Task<ActionResult<GenericResponse<UserRoleDto>>> AssignUserRole(SetUserRoleDto setUserRoleDto)
        {
            var result = await _userService.AssignUserRoleAsync(setUserRoleDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateUserRole")]
        public async Task<ActionResult<GenericResponse<UserRoleDto>>> UpdateUserRole(SetUserRoleDto setUserRoleDto)
        {
            var result = await _userService.UpdateUserRoleAsync(setUserRoleDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<GenericResponse<PaginatedResponse<GetAllUsersDto>>>> GetAllUsers([FromQuery] UserQueryParams userQueryParams)
        {
            var result = await _userService.GetAllUsersAsync(userQueryParams);
            return Ok(result);
        }
    }
}
