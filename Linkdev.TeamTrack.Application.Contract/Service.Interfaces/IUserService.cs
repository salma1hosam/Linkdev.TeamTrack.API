using Linkdev.TeamTrack.Contract.DTOs.UserDtos;
using Linkdev.TeamTrack.Contract.Responses;

namespace Linkdev.TeamTrack.Contract.Service.Interfaces
{
    public interface IUserService
    {
        Task<GenericResponse<UserDto>> RegisterAsync(RegisterDto registerDto);
        Task<GenericResponse<UserDto>> LoginAsync(LoginDto loginDto);
        Task<GenericResponse<bool>> LogOutAsync();
        Task<GenericResponse<UserRoleDto>> AssignOrUpdateUserRoleAsync(SetUserRoleDto setUserRoleDto);
    }
}
