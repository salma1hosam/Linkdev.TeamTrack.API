using Linkdev.TeamTrack.Contract.DTOs.UserDtos;
using Linkdev.TeamTrack.Core.Responses;

namespace Linkdev.TeamTrack.Contract.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(RegisterDto registerDto);
        Task<UserDto> LoginAsync(LoginDto loginDto);
        Task<string> LogOutAsync();
        Task<UserRoleDto> AssignUserRoleAsync(SetUserRoleDto setUserRoleDto);
        Task<UserRoleDto> UpdateUserRoleAsync(SetUserRoleDto setUserRoleDto);
        Task<PaginatedResponse<GetAllUsersDto>> GetAllUsersAsync(UserFilterParams userFilterParams);
        Task<IEnumerable<GetAllUsersInRoleDto>> GetAllProjectManagersAsync();
        Task<IEnumerable<GetAllUsersInRoleDto>> GetAllTeamMembersAsync();
    }
}
