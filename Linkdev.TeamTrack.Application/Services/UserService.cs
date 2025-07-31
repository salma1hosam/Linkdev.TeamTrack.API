using Linkdev.TeamTrack.Contract.DTOs.UserDtos;
using Linkdev.TeamTrack.Contract.Responses;
using Linkdev.TeamTrack.Contract.Service.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Linkdev.TeamTrack.Application.Services
{
    public class UserService(UserManager<TeamTrackUser> _userManager,
                            SignInManager<TeamTrackUser> _signInManager,
                            RoleManager<IdentityRole> _roleManager,
                            IConfiguration _configuration) : IUserService
    {
        public async Task<GenericResponse<UserRoleDto>> AssignOrUpdateUserRoleAsync(SetUserRoleDto setUserRoleDto)
        {
            var genericResponse = new GenericResponse<UserRoleDto>();
            var user = await _userManager.FindByIdAsync(setUserRoleDto.UserId);
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "User is Not Found";
                return genericResponse;
            }

            if (!await _roleManager.RoleExistsAsync(setUserRoleDto.Role))
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Role does Not Exist";
                return genericResponse;
            }

            //In Update Case
            var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (currentRole is not null)
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
                if (!removeResult.Succeeded)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Failed to Remove the user's old Role";
                    return genericResponse;
                }
            }

            //In Assign Case
            var result = await _userManager.AddToRoleAsync(user, setUserRoleDto.Role);
            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Failed to assign user role";
                return genericResponse;
            }

            var userRoleDto = new UserRoleDto()
            {
                UserId = user.Id,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Role Assigned Successfully";
            genericResponse.Data = userRoleDto;
            return genericResponse;
        }

        public async Task<GenericResponse<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var genericResponse = new GenericResponse<UserDto>();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "User is Not Found";
                return genericResponse;
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid email or password. Please check your details and try again.";
                return genericResponse;
            }
            var userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                CreatedDate = user.CreatedDate,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                Token = await CreateTokenAsync(user)
            };

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "User Loged-in Successfully";
            genericResponse.Data = userDto;
            return genericResponse;
        }

        public async Task<GenericResponse<bool>> LogOutAsync()
        {
            await _signInManager.SignOutAsync();
            return new GenericResponse<bool>()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "User Loged-out successfully",
                Data = true
            };
        }

        public async Task<GenericResponse<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            var genericResponse = new GenericResponse<UserDto>();
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Email already exists";
                return genericResponse;
            }

            var user = new TeamTrackUser()
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Failed to Create the User";
                return genericResponse;
            }

            var userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                CreatedDate = user.CreatedDate,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                Token = await CreateTokenAsync(user)
            };

            genericResponse.StatusCode = StatusCodes.Status201Created;
            genericResponse.Message = "User Created Successfully";
            genericResponse.Data = userDto;
            return genericResponse;

        }

        private async Task<string> CreateTokenAsync(TeamTrackUser user)
        {
            var Claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName , user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                Claims.Add(new Claim(ClaimTypes.Role, role));

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

            var credintials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _configuration["JWT:Issuer"],
                                             audience: _configuration["JWT:Audienece"],
                                             claims: Claims,
                                             expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:ExpireInDays"])),
                                             signingCredentials: credintials
                                             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
