using StudentManagementAPI.DTOs;

namespace StudentManagementAPI.Services
{
    public interface IAuthService
    {
        AuthResponseDto? Login(LoginDto dto);
    }
}
