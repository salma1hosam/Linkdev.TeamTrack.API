using Linkdev.TeamTrack.Contract.DTOs.UserDtos;
using Linkdev.TeamTrack.Core.Responses;
using Linkdev.TeamTrack.Contract.Application.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Linkdev.TeamTrack.Contract.Exceptions;

namespace Linkdev.TeamTrack.Application.Services
{
    public class UserService(UserManager<TeamTrackUser> _userManager,
                            SignInManager<TeamTrackUser> _signInManager,
                            RoleManager<IdentityRole> _roleManager,
                            IConfiguration _configuration) : IUserService
    {
        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
                throw new BadRequestException("Email already exists");

            var user = new TeamTrackUser()
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(E => E.Description).ToList();
                throw new BadRequestException(errors);
            }

            return new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                CreatedDate = user.CreatedDate,
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault() ?? "Unassigned",
                Token = await CreateTokenAsync(user)
            };
        }

        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email) ?? throw new NotFoundException("User is Not Found");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid email or password. Please check your details and try again");

            return new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                CreatedDate = user.CreatedDate,
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault() ?? "Unassigned",
                Token = await CreateTokenAsync(user)
            };
        }

        public async Task<string> LogOutAsync()
        {
            await _signInManager.SignOutAsync();
            return "User Loged-out successfully";
        }

        public async Task<UserRoleDto> AssignUserRoleAsync(SetUserRoleDto setUserRoleDto)
        {
            var user = await _userManager.FindByIdAsync(setUserRoleDto.UserId)
                ?? throw new NotFoundException("User is Not Found");

            var currentRole = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (currentRole is not null)
                throw new BadRequestException("This User has already been assigned to role");

            if (!await _roleManager.RoleExistsAsync(setUserRoleDto.Role))
                throw new NotFoundException("Role does Not Exist");

            var result = await _userManager.AddToRoleAsync(user, setUserRoleDto.Role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(E => E.Description).ToList();
                throw new BadRequestException(errors);
            }

            return new UserRoleDto()
            {
                UserId = user.Id,
                Email = user.Email,
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault() ?? "Unassigned"
            };
        }

        public async Task<UserRoleDto> UpdateUserRoleAsync(SetUserRoleDto setUserRoleDto)
        {
            var user = await _userManager.FindByIdAsync(setUserRoleDto.UserId)
                ?? throw new NotFoundException("User is Not Found");

            if (!await _roleManager.RoleExistsAsync(setUserRoleDto.Role))
                throw new NotFoundException("Role does Not Exist");

            var currentRole = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (currentRole is not null)
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
                if (!removeResult.Succeeded)
                {
                    var errors = removeResult.Errors.Select(E => E.Description).ToList();
                    throw new BadRequestException(errors);
                }
            }

            var result = await _userManager.AddToRoleAsync(user, setUserRoleDto.Role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(E => E.Description).ToList();
                throw new BadRequestException(errors);
            }

            return new UserRoleDto()
            {
                UserId = user.Id,
                Email = user.Email,
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault() ?? "Unassigned"
            };
        }

        public async Task<PaginatedResponse<GetAllUsersDto>> GetAllUsersAsync(UserFilterParams userFilterParams)
        {
            var TotalCount = await _userManager.Users.CountAsync();

            var paginatedResponse = new PaginatedResponse<GetAllUsersDto>
            {
                TotalCount = TotalCount,
                PageNumber = userFilterParams.PageNumber,
                PageSize = userFilterParams.PageSize
            };

            var allUsersFiltered = await _userManager.Users
                          .Where(U => (userFilterParams.UserName.IsNullOrEmpty() || U.UserName.ToLower().Contains(userFilterParams.UserName.ToLower()))
                                 && (!userFilterParams.CreatedDateFrom.HasValue || U.CreatedDate >= userFilterParams.CreatedDateFrom.Value)
                                 && (!userFilterParams.CreatedDateTo.HasValue || U.CreatedDate <= userFilterParams.CreatedDateTo.Value)
                                 && (!(userFilterParams.CreatedDateFrom.HasValue && userFilterParams.CreatedDateTo.HasValue) ||
                                      (U.CreatedDate >= userFilterParams.CreatedDateFrom.Value && U.CreatedDate <= userFilterParams.CreatedDateTo.Value)))
                          .OrderByDescending(U => U.CreatedDate)
                          .ToListAsync();

            if (allUsersFiltered?.Any() == false)
            {
                paginatedResponse.Message = "There is no data to be displayed";
                return paginatedResponse;
            }

            var allUsersDto = new List<GetAllUsersDto>();
            foreach (var user in allUsersFiltered)
            {
                allUsersDto.Add(new GetAllUsersDto()
                {
                    UserName = user.UserName,
                    CreatedDate = user.CreatedDate,
                    Email = user.Email,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault() ?? "Unassigned"
                });
            }

            paginatedResponse.Data = allUsersDto.Skip((userFilterParams.PageNumber - 1) * userFilterParams.PageSize)
                                                .Take(userFilterParams.PageSize)
                                                .ToList();
            return paginatedResponse;
        }

        public async Task<IEnumerable<GetAllUsersInRoleDto>> GetAllProjectManagersAsync()
        {
            var projectManagerUsers = await _userManager.GetUsersInRoleAsync("Project Manager");

            return projectManagerUsers.Select(U => new GetAllUsersInRoleDto
            {
                Id = U.Id,
                UserName = U.UserName
            });
        }

        public async Task<IEnumerable<GetAllUsersInRoleDto>> GetAllTeamMembersAsync()
        {
            var teamMemberUsers = await _userManager.GetUsersInRoleAsync("Team Member");

            return teamMemberUsers.Select(U => new GetAllUsersInRoleDto()
            {
                Id = U.Id,
                UserName = U.UserName
            });
        }

        private async Task<string> CreateTokenAsync(TeamTrackUser user)
        {
            var Claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName , user.UserName),
                new Claim(ClaimTypes.NameIdentifier , user.Id)
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
