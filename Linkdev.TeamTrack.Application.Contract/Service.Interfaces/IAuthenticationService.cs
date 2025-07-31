using Linkdev.TeamTrack.Contract.DTOs.AuthDtos;
using Linkdev.TeamTrack.Contract.Responses;

namespace Linkdev.TeamTrack.Contract.Service.Interfaces
{
    public interface IAuthenticationService
    {
        Task<GenericResponse<UserDto>> RegisterAsync(RegisterDto registerDto);
        Task<GenericResponse<UserDto>> LoginAsync(LoginDto loginDto);
        Task<GenericResponse<bool>> LogOutAsync();
    }
}
