using Linkdev.TeamTrack.Contract.DTOs.UserDtos;
using Linkdev.TeamTrack.Core.Responses;

namespace Linkdev.TeamTrack.Contract.Service.Interfaces
{
    public interface IUserService
    {
        Task<GenericResponse<UserDto>> RegisterAsync(RegisterDto registerDto);
        Task<GenericResponse<UserDto>> LoginAsync(LoginDto loginDto);
        Task<GenericResponse<bool>> LogOutAsync();
        Task<GenericResponse<UserRoleDto>> AssignUserRoleAsync(SetUserRoleDto setUserRoleDto);
        Task<GenericResponse<UserRoleDto>> UpdateUserRoleAsync(SetUserRoleDto setUserRoleDto);
        Task<GenericResponse<PaginatedResponse<GetAllUsersDto>>> GetAllUsersAsync(UserQueryParams userQueryParams);
        Task<GenericResponse<IEnumerable<GetAllUsersInRoleDto>>> GetAllProjectManagersAsync();
        Task<GenericResponse<IEnumerable<GetAllUsersInRoleDto>>> GetAllTeamMembersAsync();
    }
}
